// SPDX-FileCopyrightText: 2022 Francesco <frafonia@gmail.com>
// SPDX-FileCopyrightText: 2023 DrSmugleaf <DrSmugleaf@users.noreply.github.com>
// SPDX-FileCopyrightText: 2024 Errant <35878406+Errant-4@users.noreply.github.com>
// SPDX-FileCopyrightText: 2025 taydeo <td12233a@gmail.com>
//
// SPDX-License-Identifier: MIT

using Content.Server.Atmos.EntitySystems;
using Content.Server.Body.Systems;
using Content.Server.Medical.Components;
using Content.Shared.Medical.Cryogenics;

namespace Content.Server.Medical
{
    public sealed partial class CryoPodSystem
    {
        public override void InitializeInsideCryoPod()
        {
            base.InitializeInsideCryoPod();
            // Atmos overrides
            SubscribeLocalEvent<InsideCryoPodComponent, InhaleLocationEvent>(OnInhaleLocation);
            SubscribeLocalEvent<InsideCryoPodComponent, ExhaleLocationEvent>(OnExhaleLocation);
            SubscribeLocalEvent<InsideCryoPodComponent, AtmosExposedGetAirEvent>(OnGetAir);
        }

        #region Atmos handlers

        private void OnGetAir(EntityUid uid, InsideCryoPodComponent component, ref AtmosExposedGetAirEvent args)
        {
            if (TryComp<CryoPodAirComponent>(Transform(uid).ParentUid, out var cryoPodAir))
            {
                args.Gas = cryoPodAir.Air;
                args.Handled = true;
            }
        }

        private void OnInhaleLocation(EntityUid uid, InsideCryoPodComponent component, InhaleLocationEvent args)
        {
            if (TryComp<CryoPodAirComponent>(Transform(uid).ParentUid, out var cryoPodAir))
            {
                args.Gas = cryoPodAir.Air;
            }
        }

        private void OnExhaleLocation(EntityUid uid, InsideCryoPodComponent component, ExhaleLocationEvent args)
        {
            if (TryComp<CryoPodAirComponent>(Transform(uid).ParentUid, out var cryoPodAir))
            {
                args.Gas = cryoPodAir.Air;
            }
        }

        #endregion
    }
}
