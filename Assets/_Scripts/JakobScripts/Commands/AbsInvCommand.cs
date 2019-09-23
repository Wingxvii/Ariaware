using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsInvCommand<T, U, V, W> : AbstractCommand<T, U, V, W> where T : AbsInvCommand<T, U, V, W> where U : AbsInvPermission<U, T, W, V> where V : Controller where W : Inventory
{
    public List<ChannelTypes> Channels;

    private void Update()
    {
        for (int i = SpecificReceiver.Joins.Count - 1; i >= 0; i--)
        {
            Inventory inv = SpecificReceiver.GetObj(i).SpecificActor;
            for (int j = Channels.Count - 1; j >= 0; j--)
                if (CheckChannels(Channels[j], inv.channelType))
                    FeedPermission(SpecificReceiver.GetObj(i));
        }
    }

    bool CheckChannels(ChannelTypes n1, ChannelTypes n2)
    {
        if (n1.cName == ChannelName.None || n2.cName == ChannelName.None)
            return false;
        if (n1.cName == ChannelName.All || n2.cName == ChannelName.All)
            return true;
        if (n1.cName == n2.cName && (n1.allChannelNums || n2.allChannelNums || n1.cNum == n2.cNum))
            return true;
        return false;
    }
}
