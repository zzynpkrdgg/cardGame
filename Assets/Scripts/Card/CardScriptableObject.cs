using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 1)]

public class CardScriptableObject : ScriptableObject
{
    public string cardName;

    [TextArea]
    public string actionDescription, cardLore;

    public int currentHealth, attackPower, manaCost;
    public Sprite character;
    public enum cardSkills { none, drawCardOnPlay, attackAllEnemies, drawCardOnAttack, allenTheAlien, babyDucks, buffAllies, lifeSteal, omniman, kai, lloyd, gunter, 
    invincible, gumball, spiderman, anais, naruto, idaho, drawCardOnDeath, finn, bmo, healYourself, captainK, ben10, atomEve, flapjack, rick, mordecai}
    public cardSkills cardsSkill;

    public bool hasOverwhelm;
    public int buffValue;
}
