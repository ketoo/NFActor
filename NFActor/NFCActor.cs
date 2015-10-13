//-----------------------------------------------------------------------
// <copyright file="NFCActor.cs">
//     Copyright (C) 2015-2015 lvsheng.huang <https://github.com/ketoo/NFActor>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace NFrame
{
    public class NFCActor : NFIActor
    {
        public NFCActor(NFIDENTID xID, NFIActorMng xActorMng)
        {
            mxID = xID;
            mxActorMng = xActorMng;
            mxComponentMng = new NFCComponentMng();
        }

        public override NFIDENTID GetAddress()
        {
            return mxID;
        }

        public override int GetNumQueuedMessages()
        {
            return mxMessageQueue.Count;
        }

        //RegisterHandler
        public override bool Execute(float fLastFrametime, float fStartedTime)
        {
            NFIActorMessage xMsg;
            while (mxMessageQueue.TryDequeue(out xMsg) && null != xMsg)
            {
                ProcessMessage(xMsg);
            }

            return false;
        }

        public override bool SendMsg(NFIDENTID address, NFIDENTID from, NFIActorMessage xMessage)
        {
            if (mxID == address)
            {
                PushMessages(from, xMessage);
            }
            else
            {
                NFIActorMng xActorMng = GetActorMng();
                NFIActor xActor = xActorMng.GetActor(address);
                if (null != xActor)
                {
                    xActor.PushMessages(from, xMessage);
                    return true;
                }
            }


            return false;
        }

        public override bool PushMessages(NFIDENTID from, NFIActorMessage xMessage)
        {
            xMessage.nMasterActor = mxID;
            xMessage.nFromActor = mxID;
            mxMessageQueue.Enqueue(xMessage);
            return true;
        }

        /////////////////////////////////////////////////////////////


        private static void TaskMethod(object param)
        {
            NFIActorMessage xMsg = (NFIActorMessage)param;

            Console.WriteLine("Task id:{0} ThreadID  {1}   {2}", Task.CurrentId, Thread.CurrentThread.ManagedThreadId, xMsg.data);

            NFCActorMng.Intance().SendMsg(xMsg.nFromActor, xMsg.nFromActor, xMsg);
        }

        private static void TaskMethodEnd(object param)
        {
            Console.WriteLine("Task id:{0}", Task.CurrentId);
        }

        private void ProcessMessage(NFIActorMessage xMessage)
        {

            Task xTask = new Task(TaskMethod, xMessage);
            //xTask.ContinueWith(TaskMethodEnd, xMessage);

            xTask.Start();
        }
        /////////////////////////////////////////////////////////////

        private NFIActorMng GetActorMng()
        {
            return mxActorMng;
        }

        private NFIComponentMng GetComponentMng()
        {
            return mxComponentMng;
        }


        /////////////////////////////////////////////////////////////
        private readonly NFIDENTID mxID = null;
        private readonly ConcurrentQueue<NFIActorMessage> mxMessageQueue = new ConcurrentQueue<NFIActorMessage>();
        /////////////////////////////////////////////////////////////

        private readonly NFIActorMng mxActorMng = null;
        private readonly NFIComponentMng mxComponentMng = null;

    }
}
