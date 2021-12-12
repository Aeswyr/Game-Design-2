using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FountainController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private SpriteAtlas atlas;

    [SerializeField] private GameObject hint;
    [SerializeField] private GameObject interact;
    [SerializeField] private GameObject priceTag;
    bool inactive = false;
    int buff = 0;
    // Start is called before the first frame update
    void Start()
    {
        buff = Random.Range(0, 4);
        sprite.sprite = atlas.GetSprite(buff);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        if (inactive)
            return;
        



        if (other.transform.parent != null
            && other.transform.parent.gameObject.TryGetComponent(out PlayerController player)
            && player.RemoveItem(PickupType.GEM_RED, 25)) {

            animator.SetTrigger("activate");
            hint.SetActive(false);
            interact.SetActive(false);
            priceTag.SetActive(false);
            inactive = true;

            EffectsMaster.Instance.ScreenShake(0.5f, 0.2f);
            switch (buff) {
                case 0:
                    player.SetArmor();
                    break;
                case 1:
                    player.StartJuice();
                    break;
                case 2:
                    player.StartMagnet();
                    break;
                case 3:
                    player.StartSpeed();
                    break;
            }
        }
    }
}
