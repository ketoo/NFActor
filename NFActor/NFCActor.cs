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
using System.Threading;
using System.Threading.Tasks;

namespace NFrame
{
    public class NFCActor : NFIActor
    {
        public NFCActor(NFIDENTID xID, NFIActorMng xActorMng)
        {
            mxID = xID;
            mxActorMng = xActorMng;
        }

        public override NFIDENTID GetAddress()
        {
            return mxID;
        }

        public override int GetNumQueuedMessages()
        {
            return mxMessageQueue.Count;
        }

//         public static void TaskExecuteMethod(ConcurrentQueue<NFIActorMessage> x)
//         {
//             NFIActorMessage xMsg;
//             while (x.TryDequeue(out xMsg) && null != xMsg)
//             {
//                 if (null == xMsg.xMasterHandler)
//                 {
//                     return;
//                 }
// 
//                 Task xTask = Task.Factory.StartNew(TaskMethod, xMsg);
//                 if (null != xTask)
//                 {
//                     //同步消息需要wait
//                     xTask.Wait();
//                 }
//             }
// 
//         }

        public override bool Execute()
        {
            NFIActorMessage xMsg;
            while (mxMessageQueue.TryDequeue(out xMsg) && null != xMsg)
            {
                ProcessMessage(xMsg);
            }

            return false;
        }

        public override bool RegisterHandler(Handler handler)
        {
            mxMessageHandler.Enqueue(handler);
            return true;
        }

        public override bool PushMessages(NFIDENTID from, NFIActorMessage xMessage)
        {
            xMessage.nMasterActor = mxID;
            xMessage.nFromActor = from;

            if (null != mxMessageHandler)
            {
                xMessage.xMasterHandler = new ConcurrentQueue<NFIActor.Handler>(mxMessageHandler);
            }

            if (!xMessage.bAsync)
            {
                //同步消息，也不用排队，就等吧
                ProcessMessage(xMessage);
            }
            else 
            {
                //异步消息，需要new新的msg，否则担心masteractor还需使用它
                NFIActorMessage xMsg = new NFIActorMessage(xMessage);
                mxMessageQueue.Enqueue(xMsg);

                Execute();
            }

            return true;
        }

        /////////////////////////////////////////////////////////////


        private static void TaskMethod(object param)
        {
            NFIActorMessage xMsg = (NFIActorMessage)param;

            if (null != xMsg.xMasterHandler)
            {
                foreach (Handler xHandler in xMsg.xMasterHandler)
                {
                    xHandler(xMsg);
                }
            }
        }

        private void ProcessMessage(NFIActorMessage xMessage)
        {
            if (xMessage.nMasterActor != GetAddress())
            {
                return;
            }

            if (null == xMessage.xMasterHandler)
            {
                return;
            }

            Task xTask = Task.Factory.StartNew(TaskMethod, xMessage);
            if (null != xTask && !xMessage.bAsync)
            {
                //同步消息需要wait
                xTask.Wait();
            }
        }
        /////////////////////////////////////////////////////////////

        private NFIActorMng GetActorMng()
        {
            return mxActorMng;
        }


        /////////////////////////////////////////////////////////////
        private readonly NFIDENTID mxID = null;
        private readonly ConcurrentQueue<NFIActorMessage> mxMessageQueue = new ConcurrentQueue<NFIActorMessage>();
        private readonly ConcurrentQueue<Handler> mxMessageHandler = new ConcurrentQueue<Handler>();
        /////////////////////////////////////////////////////////////

        private readonly NFIActorMng mxActorMng = null;
        private readonly Task mxTask = null;

    }
}
