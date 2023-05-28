namespace Core
{
    public class CharacterUpdate : Update
    {
        public override UpdateType Type => UpdateType.Character;
        public ulong CharacterId { get; private set; }
        public CharacterUpdate(ulong characterId)
        {
            CharacterId = characterId;
        }
    }
}