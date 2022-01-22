using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace TTW.Combat
{
    public enum SelectionState
    {
        ActorSelect,
        MovementSelect,
        BenchSelect,
        AbilitySelect,
        TargetSelect,
        Animating
    }

    public class SelectionController : MonoBehaviour
    {
        [SerializeField] private SelectionState selectionState;
        [SerializeField] private ActorEntity selectedActor;
        [SerializeField] private ActorEntity selectedBenchActor;
        [SerializeField] private Ability selectedAbility;
        [SerializeField] private List<Targetable> selectedTargets;
        [SerializeField] private Cell selectedCell;
        [SerializeField] private Cell lastSelectedCell;
        [SerializeField] private bool _freezeForMovement;
        [SerializeField] private bool _freezeForAnimation;
        [SerializeField] private Pointer _pointer;
        [SerializeField] private Canvas canvas;
        [SerializeField] private ActorProfile actorProfile;
        [SerializeField] private GameObject gameOverText;
        [SerializeField] SelectionText _SelectState;


        [SerializeField] Cell[] allCells;

        ActorSelection actorSelection;
        MovementSelection movementSelection;
        BenchSelection benchSelection;
        AbilitySelection abilitySelection;
        TargetSelection targetSelection;
        AnimationController animationController;
        CombatUnloader unloader;
        List<Pointer> tempPointers = new List<Pointer>();
        

        private void Awake()
        {
            actorSelection = GetComponent<ActorSelection>();
            movementSelection = GetComponent<MovementSelection>();
            benchSelection = GetComponent<BenchSelection>();
            abilitySelection = GetComponent<AbilitySelection>();
            targetSelection = GetComponent<TargetSelection>();

            animationController = AnimationController.singleton;
            selectionState = SelectionState.ActorSelect;
            unloader = FindObjectOfType<CombatUnloader>();

            actorSelection.onActorAvailable += ActorSelection_onActorAvailable;
        }

        private void ActorSelection_onActorAvailable(object sender, EventArgs e)
        {
            SelectActor(actorSelection.SelectNextActor(DirectionTypes.Right));
        }

        private void Start()
        {
            animationController.OnAnimationFreeze += AnimationController_OnAnimationFreeze;
            animationController.OnAnimationFreezeEnd += AnimationController_OnAnimationFreezeEnd;
        }

        private void AnimationController_OnAnimationFreezeEnd(object sender, EventArgs e)
        {
            _freezeForAnimation = false;
            UpdatePointer();
        }

        private void AnimationController_OnAnimationFreeze(object sender, EventArgs e)
        {
            _freezeForAnimation = true;
            ResetAllHighlights();
        }

        private void Update()
        {
            if (_freezeForAnimation) return;

            switch (selectionState)
            {
                case SelectionState.ActorSelect:

                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        if (selectedActor == null) return;

                        if (!CheckIfAlive(selectedActor))
                        {
                            if (!unloader.CheckBenchVacant())
                            {
                                if (unloader.ActorsOnBench().Count > 0)
                                {
                                    ChangeSelectionState(SelectionState.BenchSelect);
                                    DeathSwapHighlight();
                                }   
                                else
                                {
                                    return;
                                }   
                            }   
                        }
                        else
                        {
                            ChangeSelectionState(SelectionState.MovementSelect);
                            UpdatePointer();
                        }    
                    }

                    if (actorSelection.GetTargetableActors().Count() == 0) return;

                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                        SelectActor(actorSelection.HighlightActor(DirectionTypes.Left));

                    if (Input.GetKeyDown(KeyCode.RightArrow))
                        SelectActor(actorSelection.HighlightActor(DirectionTypes.Right));

                    if (Input.GetKeyDown(KeyCode.UpArrow))
                        SelectActor(actorSelection.HighlightActor(DirectionTypes.Up));

                    if (Input.GetKeyDown(KeyCode.DownArrow))
                        SelectActor(actorSelection.HighlightActor(DirectionTypes.Down));
                    break;

                case SelectionState.MovementSelect:

                    if (Input.GetKeyDown(KeyCode.A))
                        if (movementSelection.ActorUnderPointer(selectedActor))
                            ChangeSelectionState(SelectionState.AbilitySelect);
                        else
                            movementSelection.Move(selectedActor);

                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        ChangeSelectionState(SelectionState.ActorSelect);
                        SelectActor(actorSelection.SelectNextActor(DirectionTypes.Right));
                    }
                        

                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        lastSelectedCell = selectedCell;
                        selectedCell = movementSelection.HighlightCellByDirection(DirectionTypes.Up, performHighlight: true);
                        UpdatePointer();
                    }
                        
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        lastSelectedCell = selectedCell;
                        selectedCell = movementSelection.HighlightCellByDirection(DirectionTypes.Down, performHighlight: true);
                        UpdatePointer();
                        if (selectedCell.IsBenchCell && benchSelection.IsBenchOccupied())
                            ChangeSelectionState(SelectionState.BenchSelect);
                    }
                        
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        lastSelectedCell = selectedCell;
                        selectedCell = movementSelection.HighlightCellByDirection(DirectionTypes.Left, performHighlight: true);
                        UpdatePointer();
                    }
                        
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        lastSelectedCell = selectedCell;
                        selectedCell = movementSelection.HighlightCellByDirection(DirectionTypes.Right, performHighlight: true);
                        UpdatePointer();
                    }
                        
                    break;

                case SelectionState.BenchSelect:
                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        if (selectedActor.GetComponent<StatsHandler>().Alive)
                            selectedBenchActor = benchSelection.SwapParty(selectedActor, selectedBenchActor);
                        else
                            selectedBenchActor = benchSelection.ReplaceDeadParty(selectedActor, selectedBenchActor);

                        Initialize();
                        selectedBenchActor.GetComponent<Cooldown>().SetCooldown(3f);
                        ChangeSelectionState(SelectionState.ActorSelect);
                        SelectActor(actorSelection.SelectNextActor(DirectionTypes.Right));
                    }

                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        if (selectedActor.GetComponent<StatsHandler>().Alive)
                        {
                            ChangeSelectionState(SelectionState.MovementSelect);
                            selectedCell = movementSelection.HighlightCell(lastSelectedCell);
                            UpdatePointer();
                        } 
                        else
                        {
                            ChangeSelectionState(SelectionState.ActorSelect);
                            SelectActor(actorSelection.SelectNextActor(DirectionTypes.Right));
                        }
                            
                    }

                    if (Input.GetKeyUp(KeyCode.UpArrow))
                    {
                        if (selectedActor.GetComponent<StatsHandler>().Alive)
                        {
                            ChangeSelectionState(SelectionState.MovementSelect);
                            selectedCell = movementSelection.HighlightCell(lastSelectedCell);
                            UpdatePointer();
                        } 
                    }
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                        selectedBenchActor = benchSelection.MoveThroughBench(DirectionTypes.Left);
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                        selectedBenchActor = benchSelection.MoveThroughBench(DirectionTypes.Right);

                    break;

                case SelectionState.AbilitySelect:

                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        if (!LegendaryCheck(selectedActor, abilitySelection.SelectAbility())) return;

                        selectedAbility = abilitySelection.SelectAbility();
                        targetSelection.SetActor(selectedActor);
                        targetSelection.FilterTargetables(selectedAbility);
                        ChangeSelectionState(SelectionState.TargetSelect);
                        actorProfile.AddTargetIcons(targetSelection.GetSelectedTargets(selectedAbility.targetingType));
                    }

                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        ChangeSelectionState(SelectionState.MovementSelect);
                        selectedCell = movementSelection.HighlightCellByDirection(DirectionTypes.Up, performHighlight: true);
                        UpdatePointer();
                    }

                    if (Input.GetKeyDown(KeyCode.UpArrow))
                        abilitySelection.Highlight(DirectionTypes.Up);
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                        abilitySelection.Highlight(DirectionTypes.Down);
                    break;

                case SelectionState.TargetSelect:

                    if (Input.GetKeyDown(KeyCode.A))
                    {
                        if (!targetSelection.CheckTargetsAvailable()) return;

                        targetSelection.ClearCellHighlights();
                        AttackPacket attackPacket = new AttackPacket(selectedAbility, selectedActor.GetComponent<AttackExecutor>(), selectedTargets);
                        selectedActor.GetComponent<Cooldown>().StartAbilityChannel(attackPacket, selectedAbility.attackChannelTime);
                        selectedActor = null;
                        SelectActor(actorSelection.SelectNextActor(DirectionTypes.Right));
                        ChangeSelectionState(SelectionState.ActorSelect);
                        actorProfile.ClearTargetIcons();
                    }

                    if (Input.GetKeyDown(KeyCode.S))
                    {
                        ChangeSelectionState(SelectionState.AbilitySelect);
                        actorProfile.ClearTargetIcons();
                    }

                    if (Input.GetKeyDown(KeyCode.UpArrow))
                    {
                        if (selectedTargets.Count > 1) return;
                        targetSelection.HighlightTarget(DirectionTypes.Up, selectedAbility.targetingType);
                        selectedTargets = targetSelection.GetSelectedTargets(selectedAbility.targetingType);
                        actorProfile.AddTargetIcons(targetSelection.GetSelectedTargets(selectedAbility.targetingType));
                        UpdatePointer();
                    } 
                    if (Input.GetKeyDown(KeyCode.DownArrow))
                    {
                        if (selectedTargets.Count > 1) return;
                        targetSelection.HighlightTarget(DirectionTypes.Down, selectedAbility.targetingType);
                        selectedTargets = targetSelection.GetSelectedTargets(selectedAbility.targetingType);
                        actorProfile.AddTargetIcons(targetSelection.GetSelectedTargets(selectedAbility.targetingType));
                        UpdatePointer();
                    }     
                    if (Input.GetKeyDown(KeyCode.RightArrow))
                    {
                        if (selectedTargets.Count > 1) return;
                        targetSelection.HighlightTarget(DirectionTypes.Right, selectedAbility.targetingType);
                        selectedTargets = targetSelection.GetSelectedTargets(selectedAbility.targetingType);
                        actorProfile.AddTargetIcons(targetSelection.GetSelectedTargets(selectedAbility.targetingType));
                        UpdatePointer();
                    }   
                    if (Input.GetKeyDown(KeyCode.LeftArrow))
                    {
                        if (selectedTargets.Count > 1) return;
                        targetSelection.HighlightTarget(DirectionTypes.Left, selectedAbility.targetingType);
                        selectedTargets = targetSelection.GetSelectedTargets(selectedAbility.targetingType);
                        actorProfile.AddTargetIcons(targetSelection.GetSelectedTargets(selectedAbility.targetingType));
                        UpdatePointer();
                    }

                    break;
            }
        }

        private void DeathSwapHighlight()
        {
            allCells = FindObjectsOfType<Cell>();

            print("deathswap highlight");

            var selectedCell = from c in allCells
                               where selectedActor.GridPosition == c.GetGridPos()
                               select c;

            if (selectedCell.Count() == 0) return;

            print("deathswap found cell!");

            selectedCell.ToList()[0].Highlight();
            selectedCell.ToList()[0].HighlightSelected();
        }

        //private void CharacterSwap(DirectionTypes dir)
        //{
        //    SelectionPacket packet = new SelectionPacket(selectionState, selectedAbility, selectedCell, selectedTargets);

        //    selectedActor.SaveSelectionState(packet);

        //    SelectActor(actorSelection.SelectNextActor(dir));

        //    var loadPacket = selectedActor.LoadSelectionState();

        //    selectedAbility = loadPacket.HighlightedAbility;
        //    selectedCell = loadPacket.HighlightedCell;
        //    selectedTargets = loadPacket.HighlightedTargets;

        //    if (loadPacket.SelectionState != selectionState)
        //        ChangeSelectionState(loadPacket.SelectionState);
        //    else
        //        UpdatePointer();

        //}

        private bool CheckIfAlive(ActorEntity selectedActor)
        {
            return selectedActor.GetComponent<StatsHandler>().Alive;
        }

        public void Initialize()
        {
            actorSelection.Initialize();
            SelectActor(actorSelection.HighlightActor(DirectionTypes.Left));
        }

        private bool LegendaryCheck(ActorEntity actor, Ability ability)
        {
            if(ability.legendary && actor.GetComponent<AbilitySlots>().UsedLegendary)
            {
                return false;
            }

            return true;
        }

        private void SelectActor(ActorEntity actor)
        {
            if (actor == null) return;

            if (selectedActor != null)
            {
                selectedActor.GetComponent<StatsHandler>().OnDeath -= SelectionController_OnDeath;
            }

            selectedActor = actor;

            ClearActorHighlights();

            selectedActor.SetHighlightedActor();

            RedrawActors();
            
            UpdateProfile();

            selectedActor.GetComponent<StatsHandler>().OnDeath += SelectionController_OnDeath;
        }

        private void ClearActorHighlights()
        {
            var allActors = FindObjectsOfType<ActorEntity>();

            foreach(ActorEntity a in allActors)
            {
                a.ClearHighlight();
            }
        }

        private void ChangeSelectionState(SelectionState newState)
        {
            RedrawActors();
            selectionState = newState;
            DrawSelectionUI(newState);
        }

        private void DrawSelectionUI(SelectionState newState)
        {
            animationController.EndMenuFreeze();
            movementSelection.ClearAllPathData();
            abilitySelection.ClearDisplay();
            benchSelection.ClearDisplay();
            benchSelection.ResetHighlight();
            targetSelection.ClearCellHighlights();
            ClearTargetDisplay();

            switch (newState)
            {
                case SelectionState.ActorSelect:
                    _SelectState.SetText("Actor");
                    break;
                case SelectionState.AbilitySelect:
                    animationController.StartMenuFreeze();
                    abilitySelection.ResetHighlightIndex();
                    abilitySelection.DisplayAbilities(selectedActor);
                    abilitySelection.Highlight(DirectionTypes.Up);
                    _SelectState.SetText("Ability");
                    break;
                case SelectionState.BenchSelect:
                    animationController.StartMenuFreeze();
                    benchSelection.ResetIndex();
                    benchSelection.Highlight();
                    benchSelection.DisplayActors();
                    selectedBenchActor = benchSelection.GetHighlightedBenchActor();
                    _SelectState.SetText("Wing");
                    break;
                case SelectionState.MovementSelect:
                    animationController.StartMenuFreeze();
                    movementSelection.UpdateSelection(selectedActor);
                    StartTrackingCells();
                    _SelectState.SetText("Movement");
                    break;
                case SelectionState.TargetSelect:
                    animationController.StartMenuFreeze();
                    targetSelection.HighlightTarget(DirectionTypes.Up, selectedAbility.targetingType);
                    selectedTargets = targetSelection.GetSelectedTargets(selectedAbility.targetingType);
                    _SelectState.SetText("Targeting");
                    break;
            }

            UpdatePointer();
        }

        private void StartTrackingCells()
        {
            if (selectedCell == null)
                selectedCell = movementSelection.GetHighlightedCell();
            if (lastSelectedCell == null)
                lastSelectedCell = movementSelection.GetHighlightedCell();
        }

        private void ClearTargetDisplay()
        {
            foreach(Pointer pointer in tempPointers)
            {
                Destroy(pointer.gameObject);
            }

            tempPointers.Clear();
        }

        private void UpdatePointer()
        {
            switch (selectionState)
            {
                case SelectionState.ActorSelect:
                case SelectionState.AbilitySelect:
                case SelectionState.BenchSelect:
                    _pointer.DisablePointer();
                    break;
                case SelectionState.MovementSelect:
                    _pointer.DisablePointer();

                    print("selected cell matches actor cell: " + (selectedCell.GetGridPos() == selectedActor.GridPosition));
                    print("selected cell matches movement controller's selected cell: " + (selectedCell.GetGridPos() == movementSelection.getHighlightedCellPosition));
                    
                    if (movementSelection.InvalidPath())
                    {
                        _pointer.EnablePointer();
                        _pointer.SetPointerObject(selectedActor.gameObject);
                        _pointer.ChangePointer(PointerTypes.Invalid);
                    }
                    if (movementSelection.ActorUnderPointer(selectedActor))
                    {
                        print("actor is under pointer");
                        _pointer.EnablePointer();
                        _pointer.SetPointerObject(selectedActor.gameObject);
                        _pointer.ChangePointer(PointerTypes.Confirm);
                    }
                    break;
                case SelectionState.TargetSelect:
                    _pointer.EnablePointer();
                    if (selectedTargets.Count() == 0)
                    {
                        _pointer.ChangePointer(PointerTypes.Invalid);
                        _pointer.SetPointerObject(selectedActor.gameObject);
                    }
                    if (selectedTargets.Count() >= 1)
                    {
                        if (selectedTargets[0] == null)
                        {
                            selectionState = SelectionState.ActorSelect;
                            selectedTargets.Clear();
                            return;
                        }

                        _pointer.ChangePointer(PointerTypes.Crosshair);

                        if (selectedTargets.Count > 0)
                        {
                            _pointer.SetPointerObject(selectedTargets[0].gameObject);
                        }
                    }
                    if (selectedTargets.Count() > 1)
                    {
                        for (int i = 0; i < selectedTargets.Count - 1; i++)
                        {
                            var newPointer = Instantiate(_pointer, canvas.GetComponent<RectTransform>());
                            newPointer.SetPointerObject(selectedTargets[i + 1].gameObject);
                            newPointer.ChangePointer(PointerTypes.Crosshair);
                            tempPointers.Add(newPointer);
                        }
                    }

                    if (selectedAbility.targetingType == TargetingType.random)
                    {
                        _pointer.DisablePointer();
                    }

                    break;
            }
        }

        private void UpdateProfile()
        {
            actorProfile.UpdateProfile(selectedActor);
        }

        private void RedrawActors()
        {
            if (_freezeForAnimation) return;

            var allActors = FindObjectsOfType<ActorEntity>();

            foreach(ActorEntity a in allActors)
            {
                a.Redraw();
            }
        }

        private void ResetAllHighlights()
        {
            var allActors = FindObjectsOfType<ActorEntity>();

            foreach (ActorEntity a in allActors)
            {
                a.ResetHighlights();
            }
        }

        private void SelectionController_OnDeath(object sender, EventArgs e)
        {
            ChangeSelectionState(SelectionState.ActorSelect);
        }
    }
}
