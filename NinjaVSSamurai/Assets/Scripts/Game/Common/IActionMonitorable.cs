using System;

public interface IActionMonitorable {
	event Action<bool> OnActEnd;
	void FindRegisterComponent ();
	void SetFirstAct ();
	void Dispose ();
}
