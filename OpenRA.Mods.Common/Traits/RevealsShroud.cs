#region Copyright & License Information
/*
 * Copyright 2007-2020 The OpenRA Developers (see AUTHORS)
 * This file is part of OpenRA, which is free software. It is made
 * available to you under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version. For more
 * information, see COPYING.
 */
#endregion

using System.Collections.Generic;
using System.Linq;
using OpenRA.Traits;

namespace OpenRA.Mods.Common.Traits
{
	public class RevealsShroudInfo : AffectsShroudInfo
	{
		[Desc("Stance the watching player needs to see the shroud removed.")]
		public readonly PlayerRelationship ValidStances = PlayerRelationship.Ally;

		[Desc("Can this actor reveal shroud generated by the GeneratesShroud trait?")]
		public readonly bool RevealGeneratedShroud = true;

		public override object Create(ActorInitializer init) { return new RevealsShroud(init.Self, this); }
	}

	public class RevealsShroud : AffectsShroud
	{
		readonly RevealsShroudInfo info;
		readonly Shroud.SourceType type;
		IEnumerable<int> rangeModifiers;

		public RevealsShroud(Actor self, RevealsShroudInfo info)
			: base(self, info)
		{
			this.info = info;
			type = info.RevealGeneratedShroud ? Shroud.SourceType.Visibility
				: Shroud.SourceType.PassiveVisibility;
		}

		protected override void Created(Actor self)
		{
			base.Created(self);

			rangeModifiers = self.TraitsImplementing<IRevealsShroudModifier>().ToArray().Select(x => x.GetRevealsShroudModifier());
		}

		protected override void AddCellsToPlayerShroud(Actor self, Player p, PPos[] uv)
		{
			if (!info.ValidStances.HasStance(self.Owner.RelationshipWith(p)))
				return;

			p.Shroud.AddSource(this, type, uv);
		}

		protected override void RemoveCellsFromPlayerShroud(Actor self, Player p) { p.Shroud.RemoveSource(this); }

		public override WDist Range
		{
			get
			{
				if (CachedTraitDisabled)
					return WDist.Zero;

				var range = Util.ApplyPercentageModifiers(Info.Range.Length, rangeModifiers);
				return new WDist(range);
			}
		}
	}
}
