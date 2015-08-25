namespace OpServer.Sample.Dashboard
{
  using System.Threading;

  using StackExchange.Opserver.Data;
  using StackExchange.Opserver.Data.Dashboard;
  using System;
  using System.Linq;

  class Program
  {
    static void Main(string[] args)
    {
      while (true)
      {
        UpdateMetrics();
      }
    }

    private static void UpdateMetrics()
    {
      var allNodes = DashboardData.AllNodes.Where(s => !s.IsUnwatched).Where(s => s.CPULoad >= 0 || s.MemoryUsed >= 0).ToList();
      var downNodes = allNodes.Where(x => x.Status == NodeStatus.Down);
      Thread.Sleep(3000);
      Console.Clear();

      float inBps = 0;
      float outBps = 0;
      var totalRam = 0.0;
      var totalDisk = 0.0;

      foreach (var node in allNodes)
      {
        // Let's get the metrics from VM hosts only 
        if (!node.IsVMHost)
        {
          continue;
        }

        foreach (var i in node.PrimaryInterfaces)
        {
          inBps += i.InBps.HasValue ? i.InBps.Value : 0;
          outBps += i.OutBps.HasValue ? i.OutBps.Value : 0;
        }

        totalRam += node.TotalMemory.HasValue ? node.TotalMemory.Value : 0;
        totalDisk += node.Volumes.Sum(volume => volume.Size.HasValue ? volume.Size.Value : 0);
      }

      inBps = inBps / 1024 / 1024;
      outBps = outBps / 1024 / 1024;
      totalRam = totalRam / 1024 / 1024 / 1024;
      totalDisk = totalDisk / 1024 / 1024 / 1024;

      Console.WriteLine(string.Format("You have {0} servers", allNodes.Count()));
      Console.WriteLine(string.Format("{0} of them are down", downNodes.Count()));
      Console.WriteLine(string.Format("Network Upload: {0:0.00} MB", inBps));
      Console.WriteLine(string.Format("Network Download: {0:0.00} MB", outBps));
      Console.WriteLine(string.Format("Total RAM: {0:0.00} GB", totalRam));
      Console.WriteLine(string.Format("Total HDD: {0:0.00} GB", totalDisk));
    }
  }
}
