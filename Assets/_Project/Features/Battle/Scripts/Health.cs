namespace _Project.Features.Battle.Scripts
{
    public class Health
    {
        private int _maxHp;
        private int _currentHp;
        
        public Health(int maxHp)
        {
            _maxHp = maxHp;
            _currentHp = maxHp;
        }
        
        public int CurrentHp
        {
            get => _currentHp;
            set => _currentHp = value;
        }
        
        public int MaxHp
        {
            get => _maxHp;
            set => _maxHp = value;
        }
        
        public void TakeDamage(int damage)
        {
            _currentHp -= damage;
            if (_currentHp < 0)
                _currentHp = 0;
        }
        
        public void Heal(int healAmount)
        {
            _currentHp += healAmount;
            if (_currentHp > _maxHp)
                _currentHp = _maxHp;
        }
        
        public bool IsDead()
        {
            return _currentHp <= 0;
        }
    }
}