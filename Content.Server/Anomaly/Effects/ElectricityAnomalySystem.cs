// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Nemanja <98561806+EmoGarbage404@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Slava0135 <40753025+Slava0135@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 Vasilis <vasilis@pikachu.systems>
// SPDX-FileCopyrightText: 2023 deltanedas <39013340+deltanedas@users.noreply.github.com>
// SPDX-FileCopyrightText: 2023 deltanedas <@deltanedas:kde.org>
// SPDX-FileCopyrightText: 2024 Ed <96445749+TheShuEd@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Emisse <99158783+Emisse@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Kara <lunarautomaton6@gmail.com>
// SPDX-FileCopyrightText: 2024 TemporalOroboros <TemporalOroboros@gmail.com>
// SPDX-FileCopyrightText: 2024 metalgearsloth <31366439+metalgearsloth@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Electrocution;
using Content.Server.Emp;
using Content.Server.Lightning;
using Content.Shared.Anomaly.Components;
using Content.Shared.Anomaly.Effects.Components;
using Content.Shared.StatusEffect;
using Robust.Shared.Random;
using Robust.Shared.Timing;

namespace Content.Server.Anomaly.Effects;

public sealed class ElectricityAnomalySystem : EntitySystem
{
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedTransformSystem _transform = default!;
    [Dependency] private readonly IRobustRandom _random = default!;
    [Dependency] private readonly LightningSystem _lightning = default!;
    [Dependency] private readonly ElectrocutionSystem _electrocution = default!;
    [Dependency] private readonly EmpSystem _emp = default!;
    [Dependency] private readonly EntityLookupSystem _lookup = default!;

    /// <inheritdoc/>
    public override void Initialize()
    {
        SubscribeLocalEvent<ElectricityAnomalyComponent, AnomalyPulseEvent>(OnPulse);
        SubscribeLocalEvent<ElectricityAnomalyComponent, AnomalySupercriticalEvent>(OnSupercritical);
    }

    private void OnPulse(Entity<ElectricityAnomalyComponent> anomaly, ref AnomalyPulseEvent args)
    {
        var range = anomaly.Comp.MaxElectrocuteRange * args.Stability * args.PowerModifier;

        int boltCount = (int)MathF.Floor(MathHelper.Lerp((float)anomaly.Comp.MinBoltCount, (float)anomaly.Comp.MaxBoltCount, args.Severity));

        _lightning.ShootRandomLightnings(anomaly, range, boltCount);
    }

    private void OnSupercritical(Entity<ElectricityAnomalyComponent> anomaly, ref AnomalySupercriticalEvent args)
    {
        var range = anomaly.Comp.MaxElectrocuteRange * 3 * args.PowerModifier;

        _emp.EmpPulse(_transform.GetMapCoordinates(anomaly), range, anomaly.Comp.EmpEnergyConsumption, anomaly.Comp.EmpDisabledDuration);
        _lightning.ShootRandomLightnings(anomaly, range, anomaly.Comp.MaxBoltCount * 3, arcDepth: 3);
    }

    public override void Update(float frameTime)
    {
        base.Update(frameTime);

        var query = EntityQueryEnumerator<ElectricityAnomalyComponent, AnomalyComponent, TransformComponent>();
        while (query.MoveNext(out var uid, out var elec, out var anom, out var xform))
        {
            if (_timing.CurTime < elec.NextSecond)
                continue;
            elec.NextSecond = _timing.CurTime + TimeSpan.FromSeconds(1);

            if (!_random.Prob(elec.PassiveElectrocutionChance * anom.Stability))
                continue;

            var range = elec.MaxElectrocuteRange * anom.Stability;
            var damage = (int) (elec.MaxElectrocuteDamage * anom.Severity);
            var duration = elec.MaxElectrocuteDuration * anom.Severity;

            foreach (var (ent, comp) in _lookup.GetEntitiesInRange<StatusEffectsComponent>(_transform.GetMapCoordinates(uid, xform), range))
            {
                _electrocution.TryDoElectrocution(ent, uid, damage, duration, true, statusEffects: comp, ignoreInsulation: true);
            }
        }
    }
}
