//-----------------------------------------------------------------------
// <copyright file="NFIActorMng.cs">
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
    public abstract class NFIActorMng : NFBehaviour
    {
        public abstract NFIDENTID CreateActor();

        public abstract bool ReleaseActor(NFIDENTID xID);

        public abstract void ReleaseAllActor();

        public abstract NFIActor GetActor(NFIDENTID xID);

        public abstract bool RegisterHandler(NFIDENTID xID, NFIActor.Handler handler); 

        public abstract bool SendMsg(NFIDENTID address, NFIDENTID from, NFIActorMessage xMessage);

    }
}
