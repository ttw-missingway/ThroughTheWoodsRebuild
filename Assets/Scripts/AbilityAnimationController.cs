using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TTW.Combat
{
    public enum CameraType
    {
        home,
        caster,
        target,
        field
    }

    public enum VFXType
    {
        target,
        caster,
        projectile,
        stage,
        cell
    }

    public class AbilityAnimationController : MonoBehaviour
    {
        [SerializeField] Targetable caster;
        AttackPacket packet;
        [SerializeField] List<Targetable> targets = new List<Targetable>();
        [SerializeField] AbilityCamera abilityCamera;
        AnimationController animationController;
        EventController eventController;
        AbilityUtilityTool _utilityTool;
        [SerializeField] VFX _targetVFX;
        [SerializeField] VFX _casterVFX;
        [SerializeField] VFX _projectile;
        [SerializeField] VFX _stageVFX;
        [SerializeField] VFX _cellVFX;


        private void AnimationFreeze()
        {
            animationController = FindObjectOfType<AnimationController>();
            animationController.StartAnimationFreeze();
        }

        public void SetAbilityCam()
        {
            abilityCamera = GetComponent<AbilityCamera>();
            abilityCamera.FindCameras();
            AnimationFreeze();
        }

        public void AddTarget(Targetable target)
        {
            targets.Add(target);
        }

        public void AddCaster(Targetable caster)
        {
            this.caster = caster;
        }

        public void SetCameraField()
        {
            abilityCamera.SetCamera(CameraType.field);
        }

        public void SetCameraHome()
        {
            abilityCamera.SetCamera(CameraType.home);
        }

        public void AnimationEnd()
        {
            animationController.EndAnimationFreeze();

            Destroy(gameObject);
        }

        public void SetCameraCaster()
        {
            abilityCamera.FocusCaster(caster);
            abilityCamera.SetCamera(CameraType.caster);
        }

        public void SetCameraTarget()
        {
            if (targets.Count == 0 || targets[0] == null)
            {
                abilityCamera.SetCamera(CameraType.home);
                return;
            }

            if (targets.Count < 2)
            {
                abilityCamera.SetCamera(CameraType.target);
                abilityCamera.FocusTarget(targets[0].GetComponent<Targetable>());
            } 
            else
                abilityCamera.SetCamera(CameraType.field);
        }

        public void ReceiveAttackPacket(AttackPacket packet)
        {
            this.packet = packet;

            foreach(Targetable t in packet.GetTargets())
            {
                targets.Add(t);
            }
        }

        public void SendAttackPacket()
        {
            if (targets.Count == 0) return;
            if (targets[0] == null) return;
            if (packet.AttackCancelled) return;

            foreach(Targetable t in targets)
            { 
                if (t.GetComponent<AttackReceiver>() != null)
                {
                    t.AttackReceiver.ReceiveAttackPacket(packet);
                }
                else if (t.GetComponent<Obstacle>() != null)
                {
                    t.GetComponent<Obstacle>().ReceiveAttackPacket(packet);
                }
            }
        }

        public void LoadUtilityTool(AbilityUtilityTool utilityTool)
        {
            _utilityTool = utilityTool;
        }

        public void LoadVFX(VFX vfx, VFXType type)
        {
            switch (type)
            {
                default:
                case VFXType.target: _targetVFX = vfx; break;
                case VFXType.caster: _casterVFX = vfx; break;
                case VFXType.projectile: _projectile = vfx; break;
                case VFXType.stage: _stageVFX = vfx; break;
                case VFXType.cell: _cellVFX = vfx; break;
            }
        }

        public void PerformStageVFX()
        {
            PerformVFX(VFXType.stage);
        }

        public void PerformTargetVFX()
        {
            PerformVFX(VFXType.target);
        }

        public void PerformProjectileVFX()
        {
            PerformVFX(VFXType.projectile);
        }

        public void PerformCasterVFX()
        {
            PerformVFX(VFXType.caster);
        }

        public void PerformCellVFX()
        {
            PerformVFX(VFXType.cell);
        }

        private void PerformVFX(VFXType type)
        {
            if (packet.AttackCancelled) return;

            switch (type)
            {
                default:
                case VFXType.target:
                    if (targets.Count == 0) return;
                    if (targets[0] == null) return;
                    CreateTargetVFX();
                    break;
                case VFXType.caster:
                    if (targets.Count == 0) return;
                    if (targets[0] == null) return;
                    CreateCasterVFX();
                    break;
                case VFXType.projectile:
                    if (targets.Count == 0) return;
                    if (targets[0] == null) return;
                    CreateProjectileVFX();
                    break;
                case VFXType.stage: CreateStageVFX();  break;
                case VFXType.cell: CreateCellVFX(); break;
            }
        }

        private void CreateCellVFX()
        {
            BattleGrid battleGrid = BattleGrid.singleton;
            Vector2Int[] targetCellPositions;

            if (packet.Ability.fxCellsSameAsTargetCells)
            {
                targetCellPositions = packet.Ability.gridTargetingCoordinates;
            }
            else
            {
                targetCellPositions = packet.Ability.fxCells;
            }

            foreach(Vector2Int p in targetCellPositions)
            {
                VFX vfx = Instantiate(_cellVFX, battleGrid.allCells[p].transform.position, Quaternion.identity);
                VFXLookAt(vfx);
            }
        }

        private void CreateStageVFX()
        {
            GameObject battlefield = GameObject.FindGameObjectWithTag("Battlefield");
            Vector3 offset = new Vector3(0f, 120f, 0f);
            VFX vfx = Instantiate(_stageVFX, battlefield.transform.position + offset, Quaternion.identity);
            VFXLookAt(vfx);
        }

        private void VFXLookAt(VFX vfx)
        {
            if (packet.GetTargets().Count == 0) return;

            if (packet.Ability.lookAtTarget)
            {
                vfx.TargetLookAt(packet.GetTargets()[0]);
            }
        }

        private void CreateProjectileVFX()
        {
            foreach (Targetable t in targets)
            {
                if (t.GetComponent<AttackReceiver>() == null && t.GetComponent<Obstacle>() == null)
                    continue;

                VFX vfx = Instantiate(_projectile, packet.Caster.transform.position, Quaternion.identity);
                vfx.GetComponent<Projectile>().LoadProjectile(packet.Caster.transform.position, t.transform);
                VFXLookAt(vfx);
            }
        }

        private void CreateCasterVFX()
        {
            VFX vfx = Instantiate(_casterVFX, caster.transform.position, Quaternion.identity);
            VFXLookAt(vfx);
        }

        private void CreateTargetVFX()
        {
            if (targets.Count <= 0) return;
            if (_targetVFX == null) return;

            foreach (Targetable t in targets)
            {
                if (t == null) continue;
                if (t.GetComponent<AttackReceiver>() == null && t.GetComponent<Obstacle>() == null)
                    continue;

                VFX vfx = Instantiate(_targetVFX, t.transform.position, Quaternion.identity);
                VFXLookAt(vfx);
            }
        }

        public void PerformUtilityTool()
        {
            if (packet.AttackCancelled) return;

            _utilityTool.PerformUtilities();
        }

        private void OnDestroy()
        {
            abilityCamera.SetCamera(CameraType.home);
            abilityCamera.Reset();
        }

        public void BroadcastEvent(int type)
        {
            eventController = EventController.singleton;
            eventController.BroadcastNewEventType((CombatEventType)type);
        }

        public void CasterAttackAnimation()
        {
            if (packet == null) return;
            if (caster.GetComponent<ICombatAnimation>() == null) return;

            DirectionTypes facingDirection = DirectionTypes.None;

            if (targets.Count == 1 && targets[0] != null)
            {
                if (targets[0].GetComponent<GridPosition>().GetWing() == Wing.Bow)
                {
                    facingDirection = DirectionTypes.Up;
                }
                else if (targets[0].GetComponent<GridPosition>().GetWing() == Wing.Port)
                {
                    facingDirection = DirectionTypes.Right;
                }
                else if (targets[0].GetComponent<GridPosition>().GetWing() == Wing.Starboard)
                {
                    facingDirection = DirectionTypes.Left;
                }
                else
                {
                    facingDirection = DirectionTypes.Down;
                }
            }

            if (!packet.AttackCancelled)
            {
                caster.GetComponent<ICombatAnimation>().ChangeAnimationState(packet.Ability.animationState, facingDirection);
            }
            else
            {
                VFX cancelVFX = PublicAbilities.singleton.cancelledFX;
                Instantiate(cancelVFX, caster.transform.position, Quaternion.identity);
                caster.GetComponent<ICombatAnimation>().ChangeAnimationState(CombatAnimStates.Idle, facingDirection);
            }
        }
    }
}