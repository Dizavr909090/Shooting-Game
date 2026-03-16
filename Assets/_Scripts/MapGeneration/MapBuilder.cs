using System.Collections.Generic;
using UnityEngine;
using static MapSettings;

public class MapBuilder
{
    public struct ConnectionPoint
    {
        public Coord Coord { get; private set; }
        public Vector2Int Direction { get; private set; }

        public ConnectionPoint(Coord coord, Vector2Int direction)
        {
            Coord = coord;
            Direction = direction;
        }
    }

    public static class Directions
    {
        public static readonly Vector2Int Left = new Vector2Int(-1, 0);
        public static readonly Vector2Int Right = new Vector2Int(1, 0);
        public static readonly Vector2Int Down = new Vector2Int(0, -1);
        public static readonly Vector2Int Up = new Vector2Int(0, 1);
        public static readonly Vector2Int[] All = { Left, Right, Down, Up };
    }

    private MapConfig _config;
    private TileType[,] _map;
    private bool[,] _mapFlags;
    private RectInt? _hub;
    private List<RectInt> _placedRooms = new List<RectInt>();
    private Queue<Coord> _shuffledCoords;

    public MapBuilder(MapConfig config)
    {
        _config = config;
        _map = new TileType[config.MaxGridSize, config.MaxGridSize];
        _mapFlags = new bool[config.MaxGridSize, config.MaxGridSize];
    }

    public TileType[,] Build()
    {
        FillEmpty();
        _placedRooms.Clear();

        PlaceHub();

        var potentialExits = FindHubExits();
        // Используем сид из конфига для рандома выходов
        var shuffledExits = Utility.ShuffleArray(potentialExits.ToArray(), _config.Seed);

        int branchCount = 0;
        foreach (var exit in shuffledExits)
        {
            if (branchCount >= 4) break;

            if (TryPlaceFullBranch(exit, branchCount))
            {
                branchCount++;
            }
        }

        if (_placedRooms.Count > 0)
        {
            PrepareShuffledCoords();
            PlaceObstacles();
        }

        _map = MapDecorator.AddBoundaries(_map);
        return _map;
    }

    private bool TryPlaceFullBranch(ConnectionPoint exit, int branchIndex)
    {
        int corridorLen = 6;
        int x = exit.Coord.x + exit.Direction.x;
        int y = exit.Coord.y + exit.Direction.y;

        // 1. Коридор
        RectInt corridor;
        if (exit.Direction == Directions.Up) corridor = new RectInt(x, y, 1, corridorLen);
        else if (exit.Direction == Directions.Down) corridor = new RectInt(x, y - corridorLen + 1, 1, corridorLen);
        else if (exit.Direction == Directions.Right) corridor = new RectInt(x, y, corridorLen, 1);
        else corridor = new RectInt(x - corridorLen + 1, y, corridorLen, 1);

        if (!CanPlaceRect(corridor, false, true)) return false;

        // 2. Комната
        var preset = _config.Rooms[branchIndex % _config.Rooms.Count];
        int rw = preset.Size.x;
        int rh = preset.Size.y;

        RectInt room;
        if (exit.Direction == Directions.Up)
            room = new RectInt(x - rw / 2, y + corridorLen, rw, rh);
        else if (exit.Direction == Directions.Down)
            room = new RectInt(x - rw / 2, y - corridorLen - rh + 1, rw, rh);
        else if (exit.Direction == Directions.Right)
            room = new RectInt(x + corridorLen, y - rh / 2, rw, rh);
        else
            room = new RectInt(x - corridorLen - rw + 1, y - rh / 2, rw, rh);

        if (!CanPlaceRect(room, true, false, corridor)) return false;

        // 3. РИСУЕМ
        DrawRectToMap(corridor);
        _placedRooms.Add(corridor);

        DrawRectToMap(room);
        _placedRooms.Add(room);

        // 4. ПРОБИВАЕМ СТЫКИ (чтобы MapDecorator не ставил тут стены)
        // Вход из Хаба в коридор
        _map[exit.Coord.x, exit.Coord.y] = TileType.Floor;

        // Вход из коридора в комнату (точка, где коридор упирается в край комнаты)
        if (exit.Direction == Directions.Up) _map[x, y + corridorLen] = TileType.Floor;
        else if (exit.Direction == Directions.Down) _map[x, y - corridorLen] = TileType.Floor;
        else if (exit.Direction == Directions.Right) _map[x + corridorLen, y] = TileType.Floor;
        else if (exit.Direction == Directions.Left) _map[x - corridorLen, y] = TileType.Floor;

        return true;
    }

    private bool CanPlaceRect(RectInt rect, bool withPadding, bool isCorridor, RectInt? ignoreRect = null)
    {
        // 1. Границы карты (с запасом в 2 тайла для стен)
        if (rect.xMin < 2 || rect.xMax >= _map.GetLength(0) - 2 ||
            rect.yMin < 2 || rect.yMax >= _map.GetLength(1) - 2)
            return false;

        // 2. Зона проверки
        RectInt checkArea = withPadding
            ? new RectInt(rect.x - 1, rect.y - 1, rect.width + 2, rect.height + 2)
            : rect;

        foreach (var placed in _placedRooms)
        {
            // Если это хаб и мы ставим коридор — игнорируем (они ДОЛЖНЫ пересекаться в 1 пиксель)
            if (isCorridor && _hub.HasValue && placed.Equals(_hub.Value))
                continue;

            // Игнорируем конкретный прямоугольник (например, свой же коридор для комнаты)
            if (ignoreRect.HasValue && placed.Equals(ignoreRect.Value))
                continue;

            // Самая важная часть: проверяем РЕАЛЬНОЕ пересечение (Overlap)
            // Используем чуть уменьшенный Rect, чтобы касание стенами не считалось за ошибку
            RectInt shrankPlaced = new RectInt(placed.x + 1, placed.y + 1, placed.width - 2, placed.height - 2);
            if (rect.Overlaps(shrankPlaced))
                return false;

            // Если включен padding (для комнат), проверяем наложение рамок
            if (withPadding && checkArea.Overlaps(placed))
                return false;
        }
        return true;
    }

    private void PlaceHub()
    {
        int w = (int)_config.HubSize.x;
        int h = (int)_config.HubSize.y;
        int x = _config.WorldCenter.x - (w / 2);
        int y = _config.WorldCenter.y - (h / 2);

        _hub = new RectInt(x, y, w, h);
        _placedRooms.Add(_hub.Value);

        // ВАЖНО: Рисуем пол ТОЛЬКО внутри, чтобы края остались Empty для лавы
        for (int ix = _hub.Value.xMin + 1; ix < _hub.Value.xMax - 1; ix++)
        {
            for (int iy = _hub.Value.yMin + 1; iy < _hub.Value.yMax - 1; iy++)
            {
                _map[ix, iy] = TileType.Floor;
            }
        }
    }

    private List<ConnectionPoint> FindHubExits()
    {
        var exits = new List<ConnectionPoint>();
        if (!_hub.HasValue) return exits;

        RectInt h = _hub.Value;
        for (int x = h.xMin + 1; x < h.xMax - 1; x++)
        {
            exits.Add(new ConnectionPoint(new Coord(x, h.yMax - 1), Directions.Up));
            exits.Add(new ConnectionPoint(new Coord(x, h.yMin), Directions.Down));
        }
        for (int y = h.yMin + 1; y < h.yMax - 1; y++)
        {
            exits.Add(new ConnectionPoint(new Coord(h.xMax - 1, y), Directions.Right));
            exits.Add(new ConnectionPoint(new Coord(h.xMin, y), Directions.Left));
        }
        return exits;
    }

    private void DrawRectToMap(RectInt rect)
    {
        for (int x = rect.xMin; x < rect.xMax; x++)
            for (int y = rect.yMin; y < rect.yMax; y++)
                _map[x, y] = TileType.Floor;
    }

    private void FillEmpty()
    {
        for (int x = 0; x < _map.GetLength(0); x++)
            for (int y = 0; y < _map.GetLength(1); y++)
                _map[x, y] = TileType.Empty;
    }

    private void PlaceObstacles()
    {
        int count = 0;
        if (_shuffledCoords == null || _shuffledCoords.Count == 0) return;

        for (int i = 0; i < _config.TargetObstacleCount; i++)
        {
            if (_shuffledCoords.Count == 0) break;
            Coord c = _shuffledCoords.Dequeue();

            if (c.x == _config.WorldCenterCoord.x && c.y == _config.WorldCenterCoord.y) continue;

            _map[c.x, c.y] = TileType.Obstacle;
            count++;

            // Важно: валидатор должен проверять связность всех комнат через хаб
            if (MapValidator.GetAccessibleTileCount(_map, _config.WorldCenterCoord, _mapFlags) != (GetTotalPlacedArea() - count))
            {
                _map[c.x, c.y] = TileType.Floor;
                count--;
            }
        }
    }

    private int GetTotalPlacedArea()
    {
        int total = 0;
        foreach (var r in _placedRooms)
        {
            // Если это Хаб, считаем только внутреннюю часть (без ободка стен)
            if (_hub.HasValue && r.Equals(_hub.Value))
            {
                total += (r.width - 2) * (r.height - 2);
            }
            else
            {
                total += r.width * r.height;
            }
        }
        // Добавляем 4 тайла проходов из Хаба, которые мы "пробили" вручную
        total += 4;

        return total;
    }

    private void PrepareShuffledCoords()
    {
        var coords = new List<Coord>();
        if (_hub.HasValue)
        {
            RectInt r = _hub.Value;
            // Берем только внутреннюю часть хаба (без стен), чтобы не заблокировать выходы сразу
            for (int x = r.xMin + 1; x < r.xMax - 1; x++)
                for (int y = r.yMin + 1; y < r.yMax - 1; y++)
                    coords.Add(new Coord(x, y));
        }
        _shuffledCoords = new Queue<Coord>(Utility.ShuffleArray(coords.ToArray(), _config.Seed));
    }
}