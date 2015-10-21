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

        //RegisterHandler
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

        public override bool SendMsg(NFIDENTID address, NFIDENTID from, NFIActorMessage xMessage)
        {
            if (mxID == address)
            {
                //目标就是自己
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
            xMessage.nFromActor = from;
            if (null != mxMessageHandler)
            {
                xMessage.xMasterHandler = new ConcurrentQueue<NFIActor.Handler>(mxMessageHandler);
            }

            if (!xMessage.bAsync)
            {
                //自己发送给自己，其实是同一个actor内部消息转发，不用排队
                //同步消息，也不用排队，就等吧
                ProcessMessage(xMessage);
            }
            else 
            {
                mxMessageQueue.Enqueue(xMessage);
            }

            Execute();

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
                    //循环调用，难道每次copy队列一次
                    xHandler(xMsg.nMasterActor, xMsg.nFromActor, xMsg);
                }
            }
            
            if (xMsg.bAsync)
            {
                //异步的才返回消息过去，同步的就不返回
                NFCActorMng.Intance().SendMsg(xMsg.nFromActor, xMsg.nFromActor, xMsg);
            }

        }

        private void ProcessMessage(NFIActorMessage xMessage)
        {
            if (xMessage.nMasterActor != GetAddress())
            {
                return;
            }

            Task xTask = null;
            if (xMessage.bAsync)
            {
                //异步就需要new
                NFIActorMessage x = new NFIActorMessage(xMessage);
                if (null == x.xMasterHandler)
                {
                    x.xMasterHandler = new ConcurrentQueue<NFIActor.Handler>(mxMessageHandler);
                }

                xTask = Task.Factory.StartNew(TaskMethod, x);
            }
            else
            {
                xTask = Task.Factory.StartNew(TaskMethod, xMessage);
            }
            
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

    }
}
