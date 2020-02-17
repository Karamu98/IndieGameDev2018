using UnityEngine;

namespace Game.AI
{
    public class FindPath : BTAction
    {
        public FindPath(Enemy owner) : base(owner)
        {
        }

        public override BEHAVIOUR_STATUS Update()
        {
            Vector3 result = GameManager.FindPath(GetOwner().gameObject.transform.position, GetOwner().PlayerLastPos);

            if (result == new Vector3(-1, -1, -1) || result == GetOwner().transform.position)
            {
                return BEHAVIOUR_STATUS.FAILURE;
            }
            else
            {
                //GetOwner().SetDestination(result);
                return BEHAVIOUR_STATUS.SUCCESS;
            }
        }
    }
}