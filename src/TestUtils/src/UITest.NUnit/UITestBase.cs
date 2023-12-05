﻿using System.Diagnostics;
using System.Reflection;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using UITest.Core;

namespace UITest.Appium.NUnit
{
	//#if ANDROID
	//	[TestFixture(TestDevice.Android)]
	//#elif IOSUITEST
	//	[TestFixture(TestDevice.iOS)]
	//#elif MACUITEST
	//	[TestFixture(TestDevice.Mac)]
	//#elif WINTEST
	//	[TestFixture(TestDevice.Windows)]
	//#else
	//    [TestFixture(TestDevice.iOS)]
	//    [TestFixture(TestDevice.Mac)]
	//    [TestFixture(TestDevice.Windows)]
	//    [TestFixture(TestDevice.Android)]
	//#endif
	public abstract class UITestBase : UITestContextBase
	{
		public UITestBase(TestDevice testDevice, bool useBrowserStack)
			: base(testDevice, useBrowserStack)
		{
		}

		[SetUp]
		public void TestSetup()
		{
			var name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;
			TestContext.Progress.WriteLine($">>>>> {DateTime.Now} {name} Start");

			// When running tests in isolation, we need to run the fixture setup steps for each test, navigating to the right UI
			if (RunTestsInIsolation)
			{
				SetUpForFixtureTests();
			}
		}

		[TearDown]
		public void TestTeardown()
		{
			var name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;
			TestContext.Progress.WriteLine($">>>>> {DateTime.Now} {name} Stop");

			// With BrowserStack, checking AppState isn't supported currently, producing the error below, so skip this on BrowserStack
			// BrowserStack error: System.NotImplementedException : Unknown mobile command "queryAppState". Only shell,scrollBackTo,viewportScreenshot,deepLink,startLogsBroadcast,stopLogsBroadcast,acceptAlert,dismissAlert,batteryInfo,deviceInfo,changePermissions,getPermissions,performEditorAction,startScreenStreaming,stopScreenStreaming,getNotifications,listSms,type commands are supported
			if (!_useBrowserStack)
			{
				if (App.AppState == ApplicationState.Not_Running)
				{
					// Assert.Fail will immediately exit the test which is desirable as the app is not
					// running anymore so we don't want to log diagnostic data as there is nothing to collect from
					Reset();
					FixtureSetup();
					Assert.Fail("The app was expected to be running still, investigate as possible crash");
				}
			}

			var testOutcome = TestContext.CurrentContext.Result.Outcome;
			if (testOutcome == ResultState.Error ||
				testOutcome == ResultState.Failure)
			{
				SaveDiagnosticLogs("UITestBaseTearDown");
			}

			if (RunTestsInIsolation)
			{
				TearDownDriver();
			}
		}

		protected virtual void FixtureSetup()
		{
			var name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;
			TestContext.Progress.WriteLine($">>>>> {DateTime.Now} {nameof(FixtureSetup)} for {name}");
		}

		protected virtual void FixtureTeardown()
		{
			var name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;
			TestContext.Progress.WriteLine($">>>>> {DateTime.Now} {nameof(FixtureTeardown)} for {name}");
		}

		[OneTimeSetUp]
		public void OneTimeSetup()
		{
			if (!RunTestsInIsolation)
			{
				SetUpForFixtureTests();
			}
		}

		void SetUpForFixtureTests()
		{
			InitialSetup(UITestContextSetupFixture.ServerContext);
			try
			{
				//SaveDiagnosticLogs("BeforeFixtureSetup");
				FixtureSetup();
			}
			catch
			{
				SaveDiagnosticLogs("FixtureSetup");
				throw;
			}
		}

		[OneTimeTearDown]
		public void OneTimeTearDown()
		{
			// When running tests in isolation, we can skip this. OneTimeSetup failures never happen and there's no need to
			// call FixtureTeardown to navigate back in the UI since the app will be restarted for the next test.
			if (!RunTestsInIsolation)
			{
				var outcome = TestContext.CurrentContext.Result.Outcome;

				// We only care about setup failures as regular test failures will already do logging
				if (outcome.Status == ResultState.SetUpFailure.Status &&
					outcome.Site == ResultState.SetUpFailure.Site)
				{
					SaveDiagnosticLogs("OneTimeTearDown");
				}

				FixtureTeardown();
			}
		}

		void SaveDiagnosticLogs(string? note = null)
		{
			if (string.IsNullOrEmpty(note))
				note = "-";
			else
				note = $"-{note}-";

			var logDir = (Path.GetDirectoryName(Environment.GetEnvironmentVariable("APPIUM_LOG_FILE")) ?? Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location))!;

			// App could be null if UITestContext was not able to connect to the test process (e.g. port already in use etc...)
			if (UITestContext is not null)
			{
				string name = TestContext.CurrentContext.Test.MethodName ?? TestContext.CurrentContext.Test.Name;

				var screenshotPath = Path.Combine(logDir, $"{name}-{_testDevice}{note}ScreenShot");
				_ = App.Screenshot(screenshotPath);
				// App.Screenshot appends a ".png" extension always, so include that here
				var screenshotPathWithExtension = screenshotPath + ".png";
				AddTestAttachment(screenshotPathWithExtension, Path.GetFileName(screenshotPathWithExtension));

				var pageSourcePath = Path.Combine(logDir, $"{name}-{_testDevice}{note}PageSource.txt");
				File.WriteAllText(pageSourcePath, App.ElementTree);
				AddTestAttachment(pageSourcePath, Path.GetFileName(pageSourcePath));
			}
		}

		void AddTestAttachment(string filePath, string? description = null)
		{
			try
			{
				TestContext.AddTestAttachment(filePath, description);
			}
			catch (FileNotFoundException e)
			{
				// Add the file path to better troubleshoot when these errors occur
				if (e.Message == "Test attachment file path could not be found.")
				{
					throw new FileNotFoundException($"{e.Message}: {filePath}");
				}
				else
				{
					throw;
				}
			}
		}
	}
}