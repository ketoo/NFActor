//-----------------------------------------------------------------------
// <copyright file="NFTest.cs">
//     Copyright (C) 2015-2015 lvsheng.huang <https://github.com/ketoo/NFActor>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace NFrame
{
    public class NFTest
    {
        static void Main()
        {

            NFIDENTID xID = NFCActorMng.Intance().CreateActor();
            NFIActorMessage xMsgData = new NFIActorMessage();

            xMsgData.data = "test1";
            NFCActorMng.Intance().SendMsg(xID, new NFIDENTID(), xMsgData);

            while(true)
            {
                Thread.Sleep(1);

                NFCActorMng.Intance().Execute(0.0f, 0.0f);
            }

        }
    }
}
