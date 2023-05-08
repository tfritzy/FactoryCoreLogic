namespace Core
{
    public class Quarry : Building
    {
        public override CharacterType Type => CharacterType.Quarry;
        public override int MaxEmployable => 4;

        public Quarry(Context context) : base(context)
        {
        }

        public override Schema.Character ToSchema()
        {
            throw new System.NotImplementedException();
        }
    }
}