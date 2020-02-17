public static void Main() {
  System.ServiceProcess.ServiceBase.Run(new $serviceName());
}
protected override void OnStart(string [] args) {
  // Start a child process with another copy of this script.
  try {
    Process p = new Process();
    // Redirect the output stream of the child process.
    p.StartInfo.UseShellExecute = false;
    p.StartInfo.RedirectStandardOutput = true;
    p.StartInfo.FileName = "PowerShell.exe";
    p.StartInfo.Arguments = "-c & '$scriptCopyCname' -Start";
    p.Start();
    // Read the output stream first and then wait. (Supposed to avoid deadlocks.)
    string output = p.StandardOutput.ReadToEnd();
    // Wait for the completion of the script startup code,     // which launches the -Service instance.
    p.WaitForExit();
  } catch (Exception e) {
    // Log the failure.
  }
}