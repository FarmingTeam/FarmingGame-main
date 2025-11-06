
using UnityEngine;

public class ToolAnimationController : MonoBehaviour
{

    [Header("Animation")]
    public Animator animator;

    [Header("ToolSprite")]
    [SerializeField] private ToolPivot toolPivot;
    [SerializeField] private TileReader tileReader;
    [SerializeField] private SpriteRenderer toolSprite;
    [SerializeField] private Transform toolTrans;
    [SerializeField] private GameObject BackPivot;
    [SerializeField] private GameObject LeftPivot;
    [SerializeField] private GameObject RightPivot;

    private void Update()
    {
        CheckTool();

        animator.SetBool("IsFront", tileReader.currentFacing == TileReader.Facing.Down);
        animator.SetBool("IsBack", tileReader.currentFacing == TileReader.Facing.Up);
        animator.SetBool("IsLeft", tileReader.currentFacing == TileReader.Facing.Left);
        animator.SetBool("IsRight", tileReader.currentFacing == TileReader.Facing.Right);

        Checkpos();
    }

    public void Checkpos()
    {
        // 현재 애니메이션 상태 정보 가져오기
        AnimatorClipInfo[] clipInfo = animator.GetCurrentAnimatorClipInfo(0);
        string clipName = clipInfo[0].clip.name;

        if ((clipName.Contains("Idle") || clipName.Contains("Walk")))
        {
            if (tileReader.currentFacing == TileReader.Facing.Up)
            {
                toolSprite.transform.SetParent(BackPivot.transform);
            }
            else if (tileReader.currentFacing == TileReader.Facing.Left)
            {
                toolSprite.transform.SetParent(LeftPivot.transform);
            }
            else if (tileReader.currentFacing == TileReader.Facing.Right)
            {
                toolSprite.transform.SetParent(RightPivot.transform);
            }
            else
            {
                toolSprite.transform.SetParent(toolPivot.transform);
            }
        }
        else
        {
            toolSprite.transform.SetParent(toolPivot.transform);
        }
    }

    private void CheckTool()
    {
        var curreq = toolPivot.CurrentEquip.equipmentType;

        if (curreq == EquipmentType.Axe)
        {
            animator.SetFloat("Blend", 1);
        }
        else if (curreq == EquipmentType.SeedBasket)
        {
            animator.SetFloat("Blend", 2);
        }
        else if (curreq == EquipmentType.Hoe)
        {
            animator.SetFloat("Blend", 3);
        }
        else if (curreq == EquipmentType.Pickaxe)
        {
            animator.SetFloat("Blend", 4);
        }
        else if (curreq == EquipmentType.Sickle)
        {
            animator.SetFloat("Blend", 5);
        }
        else if (curreq == EquipmentType.WateringCan)
        {
            animator.SetFloat("Blend", 6);
        }
        else if (curreq == EquipmentType.Rod)
        {
            animator.SetFloat("Blend", 7);
        }
        else
        {
            animator.SetFloat("Blend", 0);
        }
    }

    public void ToolAction()
    {
        
        animator.SetTrigger("OnAct");
        
    }

}
