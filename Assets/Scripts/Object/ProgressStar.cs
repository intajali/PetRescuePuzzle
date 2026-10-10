using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ProgressStar : MonoBehaviour
{
    public bool IsAnimDone;

    public Image starImage;

    public Sprite defaultSprite;
    public Sprite activeSprite;

    public void ActiveStar()
    {
        starImage.sprite = activeSprite;
        PlayAnimPingPong();
    }

    public void PlayAnimPingPong()
    {
        Sequence sq = DOTween.Sequence();

        Tween t1 = transform.DOScale(new Vector3(1.4f, 1.4f, 1.4f), 0.2f);
        Tween t2 = transform.DOScale(new Vector3(0.8f, 0.8f, 0.8f), 0.2f);
        Tween t3 = transform.DOScale(Vector3.one, 0.2f);
        sq.Append(t1).Append(t2).Append(t3).OnComplete(()=>{
            IsAnimDone = true;
        });
    }

    public void Reset()
    {
        starImage.sprite = defaultSprite;
        IsAnimDone = false;
    }
}
