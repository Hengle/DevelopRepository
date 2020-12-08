public interface IActionRegistrable {
	void Register (IActionMonitorable monitorable);
	void Unregister (IActionMonitorable monitorable);
}
