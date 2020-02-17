namespace Game.AI
{
    public class MeleeEnemy : Enemy
    {
        private BTMeleeEnemy m_btMeleeEnemy;

        // Use this for initialization
        protected override void Start()
        {
            base.Start();
            m_btMeleeEnemy = new BTMeleeEnemy(this);
        }

        // Update is called once per frame
        protected override void Update()
        {
            base.Update();
            if (m_isGameActive)
            {
                m_btMeleeEnemy.Update();
            }
        }

        // Update is called once per frame
        protected override void Awake()
        {
            base.Awake();
        }
    }
}