using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SimpleSpriteSheetAnimator : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public List<Sprite> sprites = new List<Sprite>();
    public int spriteTriggerIndex;
    public UnityEvent unityEvent;
    public int spriteCount;
    public float timeBeforeNextSlide;
    public bool isActive;
    private float time;
    public bool looping;

    public void PlaySpriteAnimation(int frame = -1)
    {
        _spriteRenderer.enabled = true;
        spriteCount = frame;
        isActive = true;
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.enabled = false;
    }

    public void Update()
    {
        if(!isActive)
        {
            return;
        }

        time += Time.deltaTime;
        if(time > timeBeforeNextSlide)
        {

            spriteCount++;

            if(spriteTriggerIndex == spriteCount)
            {
                unityEvent.Invoke();
            }

            if(spriteCount >= sprites.Count)
            {
                if(!looping)
                {
                    isActive = false;
                    return;
                }

                spriteCount = -1;
            }
            _spriteRenderer.sprite = sprites[spriteCount];
            time = 0;
        }
    }


}
