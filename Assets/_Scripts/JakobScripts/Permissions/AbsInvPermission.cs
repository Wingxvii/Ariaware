using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AbsInvPermission<T, U, V, W> : AbstractPermission<T, U, V, W> where T : AbsInvPermission<T, U, V, W> where U : AbsInvCommand<U, T, W, V> where V : Inventory where W : Controller
{

}
