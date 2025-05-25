using UnityEngine;

[CreateAssetMenu(fileName = "New Card", menuName = "Card", order = 1)]

public class CardScriptableObject : ScriptableObject
{
    public string cardName;

    [TextArea]
    public string actionDescription, cardLore;

    public int currentHealth, attackPower, manaCost;
    public Sprite character;
    public enum cardSkills { none, drawCardOnPlay, attackAllEnemies, drawCardOnAttack, allenTheAlien, babyDucks, buffAllies, lifeSteal, omniman}
    public cardSkills cardsSkill;
}
