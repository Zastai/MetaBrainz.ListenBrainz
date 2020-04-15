using System.Collections.Generic;

using JetBrains.Annotations;

using MetaBrainz.Common.Json;
using MetaBrainz.ListenBrainz.Interfaces;

namespace MetaBrainz.ListenBrainz.Objects {

  [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
  internal sealed class RecentListens : JsonBasedObject, IRecentListens {

    public RecentListens(IReadOnlyList<IListen> listens, string userList) {
      this.Listens = listens;
      this.UserList = userList;
    }

    public IReadOnlyList<IListen> Listens { get; }

    public string UserList { get; }

  }

}
