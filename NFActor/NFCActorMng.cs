//-----------------------------------------------------------------------
// <copyright file="NFCActorMng.cs">
//     Copyright (C) 2015-2015 lvsheng.huang <https://github.com/ketoo/NFActor>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace NFrame
{
    public class NFCActorMng : NFIActorMng
    {
        private static NFCActorMng _instance;
        private static readonly object _syncLock = new object();
        public static NFIActorMng Intance()
        {
            if (_instance == null)
            {
                lock (_syncLock)
                {
                    if (_instance == null)
                    {
                        _instance = new NFCActorMng();
                    }
                }
            }

            return _instance;
        }


        public override NFIDENTID CreateActor()
        {
            NFIDENTID xID = new NFIDENTID(0, ++mnActorIndex);
            NFIActor xActor = new NFCActor(xID, this);

            bool bRet = mxActorDic.TryAdd(xID, xActor);
            if (bRet)
            {
                xActor.Init();
                xActor.AfterInit();

                return xID;
            }

            return null;
        }

        public override void ReleaseAllActor()
        {
            foreach (var kv in mxActorDic)
            {
                mxNeedRemove.Enqueue(kv.Key);
            }
        }

        public override bool ReleaseActor(NFIDENTID xID)
        {
            mxNeedRemove.Enqueue(xID);

            return false;
        }

        public override NFIActor GetActor(NFIDENTID xID)
        {
            NFIActor xActor = null;
            if (mxActorDic.TryGetValue(xID, out xActor))
            {
                return xActor;
            }

            return null;
        }
        public override bool SendMsg(NFIDENTID address, NFIDENTID from, NFIActorMessage xMessage)
        {
            NFIActor xActor = GetActor(address);
            if (null != xActor)
            {
                xActor.PushMessages(from, xMessage);

                return true;
            }

            return false;
        }

        public override bool Execute(float fLastFrametime, float fStartedTime)
        {
            foreach (NFIDENTID k in mxNeedRemove)
            {
                NFIActor xActor = null;
                if (mxActorDic.TryRemove(k, out xActor))
                {
                    xActor.BeforeShut();
                    xActor.Shut();

                    xActor = null;

                    return true;
                }
            }

            foreach (var kv in mxActorDic)
            {
                kv.Value.Execute(fLastFrametime, fStartedTime);
            }



            return false;
        }

        ///////////////////////////////////////////////////////

        private readonly ConcurrentDictionary<NFIDENTID, NFIActor> mxActorDic = new ConcurrentDictionary<NFIDENTID, NFIActor>();
        private readonly ConcurrentQueue<NFIDENTID> mxNeedRemove = new ConcurrentQueue<NFIDENTID>();
        private readonly ConcurrentDictionary<NFIDENTID, NFIActor> mxNeedAdd = new ConcurrentDictionary<NFIDENTID, NFIActor>();
        private int mnActorIndex = 0;
    }
}
