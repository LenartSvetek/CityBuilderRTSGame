using UnityEngine;

namespace Game.Libraries.Hoverable
{
    public class HoverManager : MonoBehaviour 
    {
        private IHoverable _currentHover;
        [SerializeField] private LayerMask _hoverMask;

        void LateUpdate() 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _hoverMask)) 
            {
                // Try to get the hoverable component from the hit object
                IHoverable newHover = hit.transform.root.GetComponent<IHoverable>();

                if (newHover != _currentHover) 
                {
                    _currentHover?.OnHoverExit(); // Tell the old one to stop
                    _currentHover = newHover;    // Swap to the new one
                    _currentHover?.OnHoverEnter(); // Tell the new one to start
                }
            } 
            else if (_currentHover != null) 
            {
                // Hit nothing, clear everything
                _currentHover.OnHoverExit();
                _currentHover = null;
            }
            
            
        }
    }
}