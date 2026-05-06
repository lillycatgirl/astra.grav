using System.Collections;
using UnityEngine;

namespace Gameplay.Shop
{
    
    
    public class ShopManager : MonoBehaviour
    {
        
        public static ShopManager Instance;
        public GameObject shopPanel;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public IEnumerator ExitShop()
        {
            yield return new WaitForSeconds(0.7f);
            shopPanel.SetActive(false);
        }
        public IEnumerator EnterShop()
        {
            yield return new WaitForSeconds(0.7f);
            shopPanel.SetActive(true);
        }
    }
}
