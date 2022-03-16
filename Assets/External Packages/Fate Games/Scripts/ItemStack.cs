using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FateGames
{
    public class ItemStack : MonoBehaviour
    {
        private List<Stackable> collection = new List<Stackable>();
        private Transform _transform;

        private void Awake()
        {
            _transform = transform;
        }
        [SerializeField] private Quaternion itemRotation;

        // Adds the item to the end of the stack.
        public void Push(Stackable newItem)
        {
            float height = 0;
            Stackable previousItem = collection.Count > 0 ? collection[0] : null;
            for (int i = 1; i < collection.Count; i++)
            {
                Stackable item = collection[i];
                height += previousItem && previousItem.StackableTag == item.StackableTag ? previousItem.IdenticalMargin : previousItem.DifferentMargin;
                previousItem = item;
            }
            if (collection.Count > 0)
                height += previousItem.StackableTag == newItem.StackableTag ? previousItem.IdenticalMargin : previousItem.DifferentMargin;
            Vector3 position = Vector3.up * height;
            collection.Add(newItem);
            newItem.Transform.parent = _transform;
            newItem.Transform.localRotation = itemRotation;
            newItem.Transform.LeanMoveLocal(position, 0.2f);
        }

        // Adds the item to the front of the stack.
        public void Insert(Stackable newItem)
        {
            newItem.Transform.parent = _transform;
            collection.Insert(0, newItem);
            newItem.Transform.localRotation = itemRotation;

            AdjustItemPositions();
        }

        public Stackable Pop()
        {
            return RemoveAt(collection.Count - 1);
        }

        public Stackable Dequeue()
        {
            Stackable result = RemoveAt(0);
            AdjustItemPositions();
            return result;
        }

        private void AdjustItemPositions()
        {
            float currentHeight = 0;
            Stackable previousItem = collection.Count > 0 ? collection[0] : null;
            previousItem?.Transform.LeanMoveLocal(Vector3.zero, 0.2f);
            for (int i = 1; i < collection.Count; ++i)
            {
                Stackable item = collection[i];
                currentHeight += previousItem.StackableTag == item.StackableTag ? previousItem.IdenticalMargin : previousItem.DifferentMargin;
                Vector3 position = Vector3.up * currentHeight;
                item.Transform.LeanMoveLocal(position, 0.2f);
                previousItem = item;
            }
        }
        private Stackable RemoveAt(int index)
        {
            if (collection.Count == 0)
            {
                Debug.LogError("Cannot remove from empty stack!", this);
                return null;
            }
            Stackable objectToPop = collection[index];
            collection.RemoveAt(index);
            objectToPop.transform.parent = null;
            return objectToPop;
        }
    }

}
