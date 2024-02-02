using System.Collections;
using System.Collections.Generic;
using PrimeTween;
using Sirenix.OdinInspector;
using UnityEngine;

public partial class SideUI : MonoBehaviour
{
    //함수
    [FoldoutGroup("Book")][Button][HorizontalGroup("Book/BookBtn")]
    public void Book_Activate()
    {
        Deco_Activate();
    }
    [FoldoutGroup("Book")][Button][HorizontalGroup("Book/BookBtn")]
    public void Book_Deactivate()
    {
        Deco_Deactivate();
    }
}
