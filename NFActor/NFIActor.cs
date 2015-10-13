//-----------------------------------------------------------------------
// <copyright file="NFIActor.cs">
//     Copyright (C) 2015-2015 lvsheng.huang <https://github.com/ketoo/NFActor>
// </copyright>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NFrame
{
    public class NFIActorMessage
    {
        public NFIActorMessage()
        {
            eType = EACTOR_MESSAGE_ID.EACTOR_UNKNOW;
            nSubMsgID = 0;
        }

	    public enum EACTOR_MESSAGE_ID
	    {
            EACTOR_UNKNOW,
		    EACTOR_INIT,
		    EACTOR_AFTER_INIT,
            EACTOR_CHECKCONFIG,
		    EACTOR_EXCUTE,
		    EACTOR_BEFORE_SHUT,
		    EACTOR_SHUT,
		    EACTOR_NET_MSG,
            EACTOR_TRANS_MSG,
		    EACTOR_LOG_MSG,
		    EACTOR_EVENT_MSG,
	    }

        public EACTOR_MESSAGE_ID eType;
	    public int nSubMsgID;
        public NFIDENTID nFromActor;
        public NFIDENTID nMasterActor;
	    public string data;
	    ////////////////////event/////////////////////////////////////////////////
// 	    public NFIDENTID self;
// 	    //////////////////////////////////////////////////////////////////////////
// 	    public EVENT_ASYNC_PROCESS_END_FUNCTOR_PTR xEndFuncptr;

    }

    public abstract class NFIActor : NFBehaviour
    {
//         public abstract NFIActorMng GetActorMng();
//         public abstract NFIComponentMng GetComponentMng();
//         public abstract NFIMailBox GetMailBox();

//             public bool AddComponent<T>()
//             {
//                 return false;
//             }
// 
//             public T GetComponent<T>()
//             {
//                 return null;
//             }
            //RegisterHandler(); 

            public abstract NFIDENTID GetAddress();
            public abstract int GetNumQueuedMessages();

            public abstract bool SendMsg(NFIDENTID address, NFIDENTID from, NFIActorMessage xMessage);
            public abstract bool PushMessages(NFIDENTID from, NFIActorMessage xMessage);
    }
}
