using System;
using System.Collections;
using UnityEngine;

namespace Custom_Ducks.NewDucks
{
    public class GucciDuck : MonoBehaviour
    {
        private DuckManager _duckManager;

        private void Start()
        {
            _duckManager = base.GetComponent<DuckManager>();
        }

        private void Update()
        {
            if (_duckManager.IsNoAction)
            {
                return;
            }
            
        }

        public IEnumerator RemoveForce()
        {
            throw new NotImplementedException();
        }
    }
}