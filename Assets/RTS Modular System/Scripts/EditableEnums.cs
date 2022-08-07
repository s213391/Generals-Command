using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This document contains all enums that can be edited without risk of breaking any part of the program
namespace RTSModularSystem
{
    //replace these with every category of player object you want to use
    //-these categories can be used for hotkeys that select every object of one category
    //-can also be used for prioritising what objects get selected and what gets ignored in a click and drag selection box
    public enum PlayerObjectType
    {
        building,
        unit,
        other
    }
}

