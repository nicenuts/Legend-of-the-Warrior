using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//生成唯一的GUID
public class DataDefination : MonoBehaviour
{
    public PersistentType persistentType;
    public string ID;
    private void OnValidate()    //会在脚本被加载或脚本的属性在检查器（Inspector）中被修改时调用,一旦编辑器中有数值的变化就会被调用
    {
        if (persistentType == PersistentType.ReadWrite)
        {
            if (ID == string.Empty)
            {
                ID = System.Guid.NewGuid().ToString();   //默认写法
            }
        }
        else
        {
            ID = string.Empty;
        }
    }
}
