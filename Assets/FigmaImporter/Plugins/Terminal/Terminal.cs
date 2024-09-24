#if UNITY_EDITOR
using System.Diagnostics; // For running the command line

public class Terminal {
    // Function to run a command line command
    public static void Run(string command, string arguments = "") {
        // Create a new process
        var process = new Process();

        // Configure the process start info
        process.StartInfo.FileName = command; // Command or file to execute (e.g., "cmd.exe" for Windows)
        process.StartInfo.Arguments = arguments; // Arguments for the command

        // These two options prevent the command line window from showing up
        process.StartInfo.CreateNoWindow = true;
        process.StartInfo.UseShellExecute = false;

        // Redirect output (optional, to capture console output)
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.RedirectStandardError = true;

        try {
            // Start the process
            process.Start();

            // Read output (if needed)
            var output = process.StandardOutput.ReadToEnd();
            var error = process.StandardError.ReadToEnd();

            // Wait for the process to finish
            process.WaitForExit();

            // // Log output and errors
            // UnityEngine.Debug.Log("Output: " + output);
            // if (!string.IsNullOrEmpty(error)) {
            //     UnityEngine.Debug.LogError("Error: " + error);
            // }
        }
        catch (System.Exception e) {
            UnityEngine.Debug.LogError("Command failed: " + e.Message);
        }
    }
}
#endif
