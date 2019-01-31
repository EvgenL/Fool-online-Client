using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

namespace Fool_online.Scripts.tests
{
    public class DoRandomMove : MonoBehaviour
    {
        public Transform targetTransform;

        //'Global' queue for animations. First one ( .Peek() ) is playing, others are waiting in queue
        public Queue<Sequence> AnimationQueue = new Queue<Sequence>();

        //Entry point
        public void PlayQueuedAnimation()
        {
            //Create paused sequence 
            var seq = DG.Tweening.DOTween.Sequence();
            seq.Pause();

            //Append everything you want
            seq.Append(targetTransform.DOLocalMoveX(50f, 1f));
            //...

            //Add to queue
            AnimationQueue.Enqueue(seq);

            //Check if this animation is first in queue
            if (AnimationQueue.Count == 1)
            {
                AnimationQueue.Peek().Play();
            }

            //Set callback 
            seq.OnComplete(OnComplete);
        }

        //Callback
        private void OnComplete()
        {
            //remove animation that was completed
            AnimationQueue.Dequeue();

            //if there's animations in queue left
            if (AnimationQueue.Count > 0)
            {
                //play next
                AnimationQueue.Peek().Play();
            }
        }
    }
}
