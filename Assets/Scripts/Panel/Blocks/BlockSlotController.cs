using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSlotController : MonoBehaviour
{
    public BlockController parentBlock;
    public BlockController childBlock;

    public bool isInPanel { get { return parentBlock.isInPanel; } }

    public bool isOccupied()
    {
        return childBlock != null;
    }

    public void setChildBlock(BlockController childBlock)
    {
        this.childBlock = childBlock;
    }

    public void removeChildBlock()
    {
        childBlock = null;
    }
}
