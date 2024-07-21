using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ITarget
{
   GameObject declareOBJ(GameObject target);   
   bool declareDeath();
}
