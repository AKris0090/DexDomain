using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProcGenScripts
{
    public class WeightedRandom<T>
    {
        private readonly List<T> _items;
        private readonly List<float> _weights;
        private float _totalWeight;

        public WeightedRandom()
        {
            _items = new List<T>();
            _weights = new List<float>();
            _totalWeight = 0;
        }

        public void Add(T item, float weight)
        {
            if (weight <= 0)
            {
                return; 
            }
            _items.Add(item);
            _weights.Add(weight);
            _totalWeight += weight;
        }

        public T GetRandom()
        {
            for (int i = 0; i < _items.Count; i++)
            {
                Debug.Log($"{_items[i]}");
                Debug.Log($"{_weights[i]}");
            }
            float random = Random.Range(0, _totalWeight);
            float weightSum = 0;

            for (int i = 0; i < _items.Count; i++)
            {
                weightSum += _weights[i];
                if (random < weightSum)
                {
                    return _items[i];
                }
            }

            return _items[^1];
        }
    }
}