using SadRogue.Primitives;
using SadRogueExample.Maps;
using SadRogueExample.Themes;
using ShaiRandom.Generators;

namespace SadRogueExample.MapObjects.Components.AI
{
    /// <summary>
    /// AI for a confused enemy.  A confused enemy will stumble around aimlessly for a given number of turns, then revert to its previous AI.
    /// If an actor occupies a tile it is randomly moving into, it will attack.
    /// </summary>
    internal class ConfusedAI : AIBase
    {
        private int _numTurns;
        private readonly AIBase _previousAI;

        public ConfusedAI(int numTurns, AIBase previousAI)
        {
            _numTurns = numTurns;
            _previousAI = previousAI;
        }

        public override void TakeTurn()
        {
            if (Parent?.CurrentMap == null) return;
            if (Parent.AllComponents.GetFirst<Combatant>().HP <= 0) return;

            // Move in a random direction
            var direction = GoRogue.Random.GlobalRandom.DefaultRNG.RandomElement(AdjacencyRule.EightWay.DirectionsOfNeighborsCache);
            GameMap.MoveOrBump(Parent, direction);
            _numTurns--;
            // If we've moved the number of turns we were confused for, revert to the old AI
            if (_numTurns <= 0)
            {
                Parent.AllComponents.Remove(this);
                Parent.AllComponents.Add(_previousAI);
                // TODO: Choose color
                Engine.GameScreen?.MessageLog.AddMessage(new($"{Parent.Name} is no longer confused!", MessageColors.ImpossibleActionAppearance));
            }
        }
    }
}
