using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FractionRelationsConfig", menuName = "Scriptable Objects/FractionRelationsConfig")]
public class FractionRelationsConfig : ScriptableObject
{
    public enum FractionType { Player, Enemy, Neutral }
    public enum RelationType { Friendly, Neutral, Hostile }

    [field: SerializeField] private List<FractionRelation> _relationsList;
    [field: SerializeField] private RelationType _baseRelation = RelationType.Hostile;

    [Serializable]
    public struct FractionRelation
    {
        public FractionType from;
        public FractionType to;
        public RelationType relation;
    }

    public RelationType GetRelation(FractionType from, FractionType to)
    {
        if (from == to) return RelationType.Friendly;

        for (int i = 0; i < _relationsList.Count; i++)
        {
            if (_relationsList[i].from == from && _relationsList[i].to == to)
            {
                return _relationsList[i].relation;
            }
        }

        return _baseRelation;
    }

    public bool IsHostile(FractionType from, FractionType to)
    { 
        return GetRelation(from, to) == RelationType.Hostile;
    }

    public void SetRelation(FractionType from, FractionType to, RelationType newRelation)
    {
        for (int i = 0; i < _relationsList.Count; i++)
        {
            if (_relationsList[i].from == from && _relationsList[i].to == to)
            {
                var temp = _relationsList[i];
                temp.relation = newRelation;
                _relationsList[i] = temp;
                return;
            }
        }

        _relationsList.Add(new FractionRelation { from = from, to = to, relation = newRelation });
    }
}
