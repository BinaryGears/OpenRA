﻿using System;
using System.Drawing;
using OpenRA.Traits;

namespace OpenRA.Mods.RA
{
	public class UnitStanceAggressiveInfo : UnitStanceInfo
	{
		public override object Create(ActorInitializer init) { return new UnitStanceAggressive(init.self, this); }
	}

	/// <summary>
	/// Inherits the Return Fire damage handler!
	/// </summary>
	public class UnitStanceAggressive : UnitStance, INotifyDamage, ISelectionColorModifier
	{
		public UnitStanceAggressive(Actor self, UnitStanceAggressiveInfo info)
			: base(self, info)
		{
		
		}

		public override string OrderString
		{
			get { return "StanceAggressive"; }
		}

		protected override void OnScan(Actor self)
		{
			if (!self.IsIdle) return;
			if (!self.HasTrait<AttackBase>()) return;

			var target = ScanForTarget(self);
			if (target == null)
				return;

			AttackTarget(self, target, false);
		}

		protected override void OnActivate(Actor self)
		{
			if (!self.HasTrait<AttackBase>()) return;

			if (self.Trait<AttackBase>().IsAttacking)
				StopAttack(self);
		}

		public virtual void Damaged(Actor self, AttackInfo e)
		{
			if (!Active) return;
			if (!self.HasTrait<AttackBase>()) return;

			ReturnFire(self, e, false); // only triggers when standing still
		}

		public override Color SelectionColor
		{
			get { return Color.Red; }
		}
	}
}