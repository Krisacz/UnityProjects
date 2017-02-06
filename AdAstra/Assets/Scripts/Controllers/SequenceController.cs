using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Scripts.Controllers
{
    public class SequenceController : MonoBehaviour
    {
        private readonly List<SChain> _chains = new List<SChain>();
        private bool _isProcessing = false;
        public static SequenceController Instance;

        void OnEnable()
        {
            Instance = this;
        }

        //Main update of a Sequencer
        public void Update()
        {
            UpdateAll(Time.deltaTime);
        }

        #region UPDATE ALL
        private void UpdateAll(float deltaTime)
        {
            if(_isProcessing) return;
            
            //Nothing to process
            if (!_chains.Any()) return;

            //Flag that we are processing chains/links etc
            _isProcessing = true;

            //Update all chains/links
            foreach (var sChain in _chains)
            {
                //Get first Link from current chain
                var link = sChain.Links.Peek();

                //Update this link if it's time link
                var timeLink = link as TimeLink;
                if (timeLink != null) timeLink.Time -= deltaTime;

                //Check if Link's start condition is met
                if (link.StartCondition.Invoke())
                {
                    //If start condition is fulfilled exceute the action
                    link.Action.Invoke();

                    //Remove any completed links
                    sChain.Links.Dequeue();
                }
            }

            //Check if there are any chains without links
            _chains.RemoveAll(x => x.Links.Count == 0);
            
            //If we fully completed updating links flag that we are done
            _isProcessing = false;
        }
        #endregion

        #region HELP METHODS - STANDARD
        //Add chain containing 1 to many links
        public void AddChain(params Link[] links)
        {
            _chains.Add(new SChain(links));
        }

        //Add one stand-along links with it's own chain
        public void AddSingleLink(Link link)
        {
            _chains.Add(new SChain(link));
        }

        //Create and return time-delayed link (NEEDS TO BE ADDED TO A CHAIN TO BE PROCESSED!)
        public Link AddTimeLink(float time, Action action)
        {
            var timeLink = new TimeLink();
            timeLink.Time = time;
            timeLink.Action = action;
            timeLink.StartCondition = () => TimeCondition(timeLink);
            return timeLink;
        }

        private bool TimeCondition(TimeLink timeLink)
        {
            foreach (var sChain in _chains)
            {
                foreach (var link in sChain.Links)
                {
                    var tLink = (link as TimeLink);
                    if (tLink != null && tLink.Equals(timeLink) && tLink.Time <= 0f) return true;
                }
            }

            return false;
        }

        public void AddSingleTimeLink(float time, Action action)
        {
            _chains.Add(new SChain(AddTimeLink(time, action)));
        }
        #endregion

        #region HELP METHODS - TWEENS
        public void AddTweenChain(Action onTweenChainCompleted, params TweenLink[] tweens)
        {
            var chain = new SChain();

            //Create final link for when tween chain is completed
            //We will add it to last tween link
            var finalLink = new TweenLink();
            finalLink.StartCondition = () => finalLink.StartTween == true;
            finalLink.Action = onTweenChainCompleted;

            //Loop throu provided tweens and "chain" them together
            for (var index = 0; index < tweens.Length; index++)
            {
                var tweenLink = tweens[index];

                //Start 1st tween immediately
                if (index == 0) tweenLink.StartTween = true;

                //Add next tween in a chain (A>B, B>C, C>D .... if last one > NULL)
                tweenLink.NextTweenLink = (index < tweens.Length - 1) ? tweens[index + 1] : finalLink;

                //Add to chain
                chain.Links.Enqueue(tweenLink);
            }

            //Enqueue final link
            chain.Links.Enqueue(finalLink);

            _chains.Add(chain);
        }

        public void OnTweenCompleted(TweenLink link)
        {
            link.StartTween = true;
        }
        #endregion
    }

    #region CHAIN
    //Sequencer Chain - Links Holder
    public class SChain
    {
        public Queue<Link> Links { get; private set; }

        public SChain(params Link[] links)
        {
            Links = new Queue<Link>(links);
        }
    }
    #endregion

    #region LINK
    //Link - part of a chain - action with starting condition
    public class Link
    {
        public Func<bool> StartCondition { get; set; }
        public Action Action { get; set; }

        public Link(Func<bool> startCondition, Action action)
        {
            StartCondition = startCondition;
            Action = action;
        }

        public Link()
        {
        }
    }
    #endregion

    #region TIME LINK
    public class TimeLink : Link
    {
        public float Time { get; set; }

        public TimeLink(Func<bool> startCondition, Action action, float time) : base(startCondition, action)
        {
            Time = time;
        }

        public TimeLink()
        {
            
        }
    }
    #endregion

    #region TWEEN LINK
    //Used as a base but also for final link in TweenChains
    public class TweenLink: Link
    {
        public bool StartTween { get; set; }
        public TweenLink NextTweenLink { get; set; }
    }
    
    public class MoveToTweenLink : TweenLink
    {
        public MoveToTweenLink(GameObject go, Vector3 to, float time, iTween.EaseType easeType)
        {
            StartTween = false;
            StartCondition = () => StartTween == true;
            Action = () =>
            {
                var hashtable = new Hashtable();
                hashtable.Add(iTweenHelp.position.ToString(), to);
                hashtable.Add(iTweenHelp.time.ToString(), time);
                hashtable.Add(iTweenHelp.easetype.ToString(), easeType);
                hashtable.Add(iTweenHelp.oncompletetarget.ToString(), SequenceController.Instance.gameObject);
                hashtable.Add(iTweenHelp.oncomplete.ToString(), "OnTweenCompleted");
                hashtable.Add(iTweenHelp.oncompleteparams.ToString(), NextTweenLink);
                iTween.MoveTo(go, hashtable);
            };
        }
    }
    #endregion

}
