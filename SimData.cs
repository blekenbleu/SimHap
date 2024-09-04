using GameReaderCommon;		// for GameData
using SimHub.Plugins;		// PluginManager
using System;				// for Math

namespace blekenbleu.Haptic
{
	public partial class SimData
	{
		public double FPS;
		public double SpeedMs;
		public double Accelerator;
		public double Clutch;
		public double Handbrake;
		public bool ABSActive;
		public double ABSPulse;
		public long ABSPauseInterval;
		public long ABSPulseInterval;
		public long ABSTicks;
		public int Gear;
		public int Gears;
		public bool Downshift;
		public bool Upshift;
		public int GearPrevious;
		public double GearInterval;
		public long ShiftTicks;
		public double CylinderDisplacement;
		public double EngineLoad;
		public int WiperStatus;
		public int CarInitCount;
		public int IdleSampleCount;			// used in Refresh()
		public int AccSamples;
		public int Acc0;
		public int Acc1;
		public double[] AccHeave;
		public double[] AccSurge;
		public double[] AccSway;
		public double AccHeave2S;
		public double AccSurge2S;
		public double AccSway2S;
		public double WorldSpeedY;
		public double JerkZ;
		public double JerkY;
		public double JerkX;
		public double AccHeaveAvg;
		public double AccSurgeAvg;
		public double AccSwayAvg;
		public double JerkYAvg;
		public double AccHeaveAbs;
		public double InvAccSurgeAvg;
		public bool Airborne;
		public double WheelLoadFL;
		public double WheelLoadFR;
		public double WheelLoadRL;
		public double WheelLoadRR;
		public double Yaw;
		public double YawPrev;
		public double YawRate;
		public double YawRateAvg;
		public double MotionPitch;
		public double MotionRoll;
		public double MotionYaw;
		public double MotionHeave;
		public double MotionSurge;
		public double MotionSway;
		public double SuspensionDistFL;
		public double SuspensionDistFR;
		public double SuspensionDistRL;
		public double SuspensionDistRR;
		public double SuspensionDistFLP;
		public double SuspensionDistFRP;
		public double SuspensionDistRLP;
		public double SuspensionDistRRP;
		public double SuspensionVelFL;
		public double SuspensionVelFR;
		public double SuspensionVelRL;
		public double SuspensionVelRR;
		public double SuspensionVelFLP;
		public double SuspensionVelFRP;
		public double SuspensionVelRLP;
		public double SuspensionVelRRP;
		public double SuspensionAccFL;
		public double SuspensionAccFR;
		public double SuspensionAccRL;
		public double SuspensionAccRR;
		public double SuspensionAccFLP;
		public double SuspensionAccFRP;
		public double SuspensionAccRLP;
		public double SuspensionAccRRP;
		public double SuspensionJerkFL;
		public double SuspensionJerkFR;
		public double SuspensionJerkRL;
		public double SuspensionJerkRR;
		public double SuspensionFL;
		public double SuspensionFR;
		public double SuspensionRL;
		public double SuspensionRR;
		public double SuspensionFront;
		public double SuspensionRear;
		public double SuspensionLeft;
		public double SuspensionRight;
		public double SuspensionAll;
		public double SuspensionAccAll;
		public bool RumbleFromPlugin;
		public double TiresLeft;
		public double TiresRight;
		public double RumbleLeftAvg;
		public double RumbleRightAvg;
		public double RumbleLeft;
		public double RumbleRight;
		public double SuspensionFreq;
		public double SuspensionFreqRa;
		public double SuspensionFreqRb;
		public double SuspensionFreqRc;
		public double SuspensionFreqR1;
		public double SuspensionFreqR2;
		public double SuspensionFreqR3;
		public double SuspensionFreqR4;
		public double SuspensionFreqR5;
		public double SuspensionMultRa;
		public double SuspensionMultRb;
		public double SuspensionMultRc;
		public double SuspensionMultR1;
		public double SuspensionMultR2;
		public double SuspensionMultR3;
		public double SuspensionMultR4;
		public double SuspensionMultR5;
		public double SuspensionRumbleMultRa;
		public double SuspensionRumbleMultRb;
		public double SuspensionRumbleMultRc;
		public double SuspensionRumbleMultR1;
		public double SuspensionRumbleMultR2;
		public double SuspensionRumbleMultR3;
		public double SuspensionRumbleMultR4;
		public double SuspensionRumbleMultR5;

		static PluginManager PM;
		private float Data(string prop)
		{
			var foo = PM.GetPropertyValue(raw+"Data."+prop);
			if (foo != null)
				return Convert.ToSingle(foo);
			return 0;
		}

		private float Physics(string prop)
		{
			var foo = PM.GetPropertyValue(raw+"Physics."+prop);
			if (foo != null)
				return Convert.ToSingle(foo);
			return 0;
		}

		private float Raw(string prop)
		{
			var foo = PM.GetPropertyValue(raw+""+prop);
			if (foo != null)
				return Convert.ToSingle(foo);
			return 0;
		}

		private void UpdateVehicle()
		{
			SuspensionDistFLP = SuspensionDistFL;
			SuspensionDistFRP = SuspensionDistFR;
			SuspensionDistRLP = SuspensionDistRL;
			SuspensionDistRRP = SuspensionDistRR;
			SuspensionDistFL = 0.0;
			SuspensionDistFR = 0.0;
			SuspensionDistRL = 0.0;
			SuspensionDistRR = 0.0;
			SuspensionVelFLP = SuspensionVelFL;
			SuspensionVelFRP = SuspensionVelFR;
			SuspensionVelRLP = SuspensionVelRL;
			SuspensionVelRRP = SuspensionVelRR;
			SuspensionVelFL = 0.0;
			SuspensionVelFR = 0.0;
			SuspensionVelRL = 0.0;
			SuspensionVelRR = 0.0;
			ABSActive = H.N.ABSActive == 1;
			bool flag = true;
			switch (BlekHapt.CurrentGame)
			{
				case GameId.AC:
					SuspensionDistFL = Physics("SuspensionTravel01");
					SuspensionDistFR = Physics("SuspensionTravel02");
					SuspensionDistRL = Physics("SuspensionTravel03");
					SuspensionDistRR = Physics("SuspensionTravel04");

					TiresLeft = 1.0 + (double) Math.Max(Physics("TyreContactHeading01.Y"), Physics("TyreContactHeading03.Y"));
					TiresRight = 1.0 + (double) Math.Max(Physics("TyreContactHeading02.Y"), Physics("TyreContactHeading04.Y"));
					if (RumbleLeftAvg == 0.0)
						RumbleLeftAvg = TiresLeft;
					if (RumbleRightAvg == 0.0)
						RumbleRightAvg = TiresRight;
					RumbleLeftAvg = (RumbleLeftAvg + TiresLeft) * 0.5;
					RumbleRightAvg = (RumbleRightAvg + TiresRight) * 0.5;
					RumbleLeft = Math.Abs(TiresLeft / RumbleLeftAvg - 1.0) * 2000.0;
					RumbleRight = Math.Abs(TiresRight / RumbleRightAvg - 1.0) * 2000.0;
					break;
				case GameId.ACC:
					SuspensionDistFL = Physics("SuspensionTravel01");
					SuspensionDistFR = Physics("SuspensionTravel02");
					SuspensionDistRL = Physics("SuspensionTravel03");
					SuspensionDistRR = Physics("SuspensionTravel04");
					WiperStatus = (int) PM.GetPropertyValue(raw+"Graphics.WiperLV");
					break;
				case GameId.AMS1:
					SuspensionDistFL = Data("wheel01.suspensionDeflection");
					SuspensionDistFR = Data("wheel02.suspensionDeflection");
					SuspensionDistRL = Data("wheel03.suspensionDeflection");
					SuspensionDistRR = Data("wheel04.suspensionDeflection");
					SpeedMs = Raw("CurrentPlayer.speed");
					break;
				case GameId.AMS2:
					SuspensionDistFL = Raw("mSuspensionTravel01");
					SuspensionDistFR = Raw("mSuspensionTravel02");
					SuspensionDistRL = Raw("mSuspensionTravel03");
					SuspensionDistRR = Raw("mSuspensionTravel04");
					break;
				case GameId.D4:
					SuspensionDistFL = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistFR = Raw("SuspensionPositionFrontRight") * 0.001;
					SuspensionDistRL = Raw("SuspensionPositionRearLeft") * 0.001;
					SuspensionDistRR = Raw("SuspensionPositionRearRight") * 0.001;
					break;
				case GameId.DR2:
					SuspensionDistFL = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistFR = Raw("SuspensionPositionFrontRight") * 0.001;
					SuspensionDistRL = Raw("SuspensionPositionRearLeft") * 0.001;
					SuspensionDistRR = Raw("SuspensionPositionRearRight") * 0.001;
					break;
				case GameId.WRC23:
					SuspensionDistFL = Raw("SessionUpdate.vehicle_hub_position_fl");
					SuspensionDistFR = Raw("SessionUpdate.vehicle_hub_position_fr");
					SuspensionDistRL = Raw("SessionUpdate.vehicle_hub_position_bl");
					SuspensionDistRR = Raw("SessionUpdate.vehicle_hub_position_br");
					SpeedMs = Raw("SessionUpdate.vehicle_speed");
					break;
				case GameId.F12022:
					SuspensionDistFL = Raw("PlayerMotionData.m_suspensionPosition01") * 0.001;
					SuspensionDistFR = Raw("PlayerMotionData.m_suspensionPosition02") * 0.001;
					SuspensionDistRL = Raw("PlayerMotionData.m_suspensionPosition03") * 0.001;
					SuspensionDistRR = Raw("PlayerMotionData.m_suspensionPosition04") * 0.001;
					break;
				case GameId.F12023:
					SuspensionDistFL = Raw("PacketMotionExData.m_suspensionPosition01") * 0.001;
					SuspensionDistFR = Raw("PacketMotionExData.m_suspensionPosition02") * 0.001;
					SuspensionDistRL = Raw("PacketMotionExData.m_suspensionPosition03") * 0.001;
					SuspensionDistRR = Raw("PacketMotionExData.m_suspensionPosition04") * 0.001;
					break;
				case GameId.Forza:
					SuspensionDistFL = Raw("SuspensionTravelMetersFrontLeft");
					SuspensionDistFR = Raw("SuspensionTravelMetersFrontRight");
					SuspensionDistRL = Raw("SuspensionTravelMetersRearLeft");
					SuspensionDistRR = Raw("SuspensionTravelMetersRearRight");
					break;
				case GameId.GTR2:
				case GameId.GSCE:
				case GameId.RF1:
					SuspensionDistFL = Data("wheel01.suspensionDeflection");
					SuspensionDistFR = Data("wheel02.suspensionDeflection");
					SuspensionDistRL = Data("wheel03.suspensionDeflection");
					SuspensionDistRR = Data("wheel04.suspensionDeflection");
					SpeedMs = Raw("CurrentPlayer.speed");
					break;
				case GameId.IRacing:
					if (PM.GetPropertyValue(raw+"Telemetry.LFshockDefl") != null)
					{
						SuspensionDistFL = Raw("Telemetry.LFshockDefl");
						SuspensionDistFR = Raw("Telemetry.RFshockDefl");
					}
					else if (PM.GetPropertyValue(raw+"Telemetry.LFSHshockDefl") != null)
					{
						SuspensionDistFL = Raw("Telemetry.LFSHshockDefl");
						SuspensionDistFR = Raw("Telemetry.RFSHshockDefl");
					}
					if (PM.GetPropertyValue(raw+"Telemetry.LRshockDefl") != null)
					{
						SuspensionDistRL = Raw("Telemetry.LRshockDefl");
						SuspensionDistRR = Raw("Telemetry.RRshockDefl");
					}
					else if (PM.GetPropertyValue(raw+"Telemetry.LRSHshockDefl") != null)
					{
						SuspensionDistRL = Raw("Telemetry.LRSHshockDefl");
						SuspensionDistRR = Raw("Telemetry.RRSHshockDefl");
					}
					if (PM.GetPropertyValue(raw+"Telemetry.CFshockDefl") != null)
					{
						SuspensionDistFL = 0.5 * SuspensionDistFL + Raw("Telemetry.CFshockDefl");
						SuspensionDistFR = 0.5 * SuspensionDistFR + Raw("Telemetry.CFshockDefl");
					}
					else if (PM.GetPropertyValue(raw+"Telemetry.HFshockDefl") != null)
					{
						SuspensionDistFL = 0.5 * SuspensionDistFL + Raw("Telemetry.HFshockDefl");
						SuspensionDistFR = 0.5 * SuspensionDistFR + Raw("Telemetry.HFshockDefl");
					}
					if (PM.GetPropertyValue(raw+"Telemetry.CRshockDefl") != null)
					{
						SuspensionDistRL = 0.5 * SuspensionDistRL + Raw("Telemetry.CRshockDefl");
						SuspensionDistRR = 0.5 * SuspensionDistRR + Raw("Telemetry.CRshockDefl");
					}
					break;
				case GameId.PC2:
					SuspensionDistFL = Raw("mSuspensionTravel01");
					SuspensionDistFR = Raw("mSuspensionTravel02");
					SuspensionDistRL = Raw("mSuspensionTravel03");
					SuspensionDistRR = Raw("mSuspensionTravel04");
					break;
				case GameId.RBR:
					SuspensionDistFL = Raw("NGPTelemetry.car.suspensionLF.springDeflection");
					SuspensionDistFR = Raw("NGPTelemetry.car.suspensionRF.springDeflection");
					SuspensionDistRL = Raw("NGPTelemetry.car.suspensionLB.springDeflection");
					SuspensionDistRR = Raw("NGPTelemetry.car.suspensionRB.springDeflection");
					break;
				case GameId.RF2:
					SuspensionDistFL = Raw("CurrentPlayerTelemetry.mWheels01.mSuspensionDeflection");
					SuspensionDistFR = Raw("CurrentPlayerTelemetry.mWheels02.mSuspensionDeflection");
					SuspensionDistRL = Raw("CurrentPlayerTelemetry.mWheels03.mSuspensionDeflection");
					SuspensionDistRR = Raw("CurrentPlayerTelemetry.mWheels04.mSuspensionDeflection");
					if (H.N.Brake > 90.0)
					{
						ABSActive = (Raw("CurrentPlayerTelemetry.mWheels01.mBrakePressure")
								   + Raw("CurrentPlayerTelemetry.mWheels02.mBrakePressure")) * 100.0 < H.N.Brake - 1.0;
						break;
					}
					break;
				case GameId.RRRE:
					if (PM.GetPropertyValue(raw+"Player.SuspensionDeflection.FrontLeft") != null)
					{
						SuspensionDistFL = Raw("Player.SuspensionDeflection.FrontLeft");
						SuspensionDistFR = Raw("Player.SuspensionDeflection.FrontRight");
					}
					if (PM.GetPropertyValue(raw+"Player.SuspensionDeflection.RearLeft") != null)
					{
						SuspensionDistRL = Raw("Player.SuspensionDeflection.RearLeft");
						SuspensionDistRR = Raw("Player.SuspensionDeflection.RearRight");
					}
					if (PM.GetPropertyValue(raw+"Player.ThirdSpringSuspensionDeflectionFront") != null)
					{
						SuspensionDistFL = 0.5 * SuspensionDistFL + Raw("Player.ThirdSpringSuspensionDeflectionFront");
						SuspensionDistFR = 0.5 * SuspensionDistFR + Raw("Player.ThirdSpringSuspensionDeflectionFront");
					}
					if (PM.GetPropertyValue(raw+"Player.ThirdSpringSuspensionDeflectionRear") != null)
					{
						SuspensionDistRL = 0.5 * SuspensionDistRL + Raw("Player.ThirdSpringSuspensionDeflectionRear");
						SuspensionDistRR = 0.5 * SuspensionDistRR + Raw("Player.ThirdSpringSuspensionDeflectionRear");
					}
					break;
				case GameId.GTL:
				case GameId.RACE07:
					double num1 = Raw("carCGLocY") * 0.2;

					SuspensionDistFL = num1 * WheelLoadFL;
					SuspensionDistFR = num1 * WheelLoadFR;
					SuspensionDistRL = num1 * WheelLoadRL;
					SuspensionDistRR = num1 * WheelLoadRR;
					break;
				case GameId.LFS:
					SuspensionDistFL = Raw("OutSim2.OSWheels01.SuspDeflect");
					SuspensionDistFR = Raw("OutSim2.OSWheels02.SuspDeflect");
					SuspensionDistRL = Raw("OutSim2.OSWheels03.SuspDeflect");
					SuspensionDistRR = Raw("OutSim2.OSWheels04.SuspDeflect");
					break;
				case GameId.WRC10:
				case GameId.WRCX:
					SuspensionDistFL = Raw("suspension_position01");
					SuspensionDistFR = Raw("suspension_position02");
					SuspensionDistRL = Raw("suspension_position03");
					SuspensionDistRR = Raw("suspension_position04");
					break;
				case GameId.WRCGen:
					SuspensionDistFL = Raw("SuspensionPositionFrontLeft");
					SuspensionDistFR = Raw("SuspensionPositionFrontRight");
					SuspensionDistRL = Raw("SuspensionPositionRearLeft");
					SuspensionDistRR = Raw("SuspensionPositionRearRight");
					break;
				case GameId.F12016:
					SuspensionDistFL = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistFR = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistRL = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistRR = Raw("SuspensionPositionFrontLeft") * 0.001;
					break;
				case GameId.F12017:
					SuspensionDistFL = Raw("m_susp_pos01") * 0.001;
					SuspensionDistFR = Raw("m_susp_pos02") * 0.001;
					SuspensionDistRL = Raw("m_susp_pos03") * 0.001;
					SuspensionDistRR = Raw("m_susp_pos04") * 0.001;
					break;
				case GameId.GLegends:
					SuspensionDistFL = Raw("SuspensionPositionFrontLeft") * 0.001;
					SuspensionDistFR = Raw("SuspensionPositionFrontRight") * 0.001;
					SuspensionDistRL = Raw("SuspensionPositionRearLeft") * 0.001;
					SuspensionDistRR = Raw("SuspensionPositionRearRight") * 0.001;
					break;
				case GameId.KK:
					flag = false;
					break;
				case GameId.ATS:
				case GameId.ETS2:
					SuspensionDistFL = Raw("TruckValues.CurrentValues.WheelsValues.SuspDeflection01");
					SuspensionDistFR = Raw("TruckValues.CurrentValues.WheelsValues.SuspDeflection02");
					SuspensionDistRL = Raw("TruckValues.CurrentValues.WheelsValues.SuspDeflection03");
					SuspensionDistRR = Raw("TruckValues.CurrentValues.WheelsValues.SuspDeflection04");
					WiperStatus = (bool) PM.GetPropertyValue(raw+"TruckValues.CurrentValues.DashboardValues.Wipers") ? 1 : 0;
					break;
				case GameId.BeamNG:
					flag = false;
					SuspensionDistFL = Raw("suspension_position_fl") * 0.05;
					SuspensionDistFR = Raw("suspension_position_fr") * 0.05;
					SuspensionDistRL = Raw("suspension_position_rl") * 0.05;
					SuspensionDistRR = Raw("suspension_position_rr") * 0.05;
					SuspensionVelFL = Raw("suspension_velocity_fl") * 0.05;
					SuspensionVelFR = Raw("suspension_velocity_fr") * 0.05;
					SuspensionVelRL = Raw("suspension_velocity_rl") * 0.05;
					SuspensionVelRR = Raw("suspension_velocity_rr") * 0.05;
					break;
				case GameId.GPBikes:
				case GameId.MXBikes:
					flag = false;
					SuspensionDistFL = Raw("m_sData.m_afSuspLength01");
					SuspensionDistFR = Raw("m_sData.m_afSuspLength01");
					SuspensionDistRL = Raw("m_sData.m_afSuspLength02");
					SuspensionDistRR = Raw("m_sData.m_afSuspLength02");
					SuspensionVelFL = Raw("m_sData.m_afSuspVelocity01");
					SuspensionVelFR = Raw("m_sData.m_afSuspVelocity01");
					SuspensionVelRL = Raw("m_sData.m_afSuspVelocity02");
					SuspensionVelRR = Raw("m_sData.m_afSuspVelocity02");
					if ((int) PM.GetPropertyValue(raw+"m_sData.m_aiWheelMaterial01") == 7
					 || (int) PM.GetPropertyValue(raw+"m_sData.m_aiWheelMaterial02") == 7)
					{
						RumbleLeft = 50.0;
						RumbleRight = 50.0;
						break;
					}
					RumbleLeft = 0.0;
					RumbleRight = 0.0;
					break;
				case GameId.LMU:
					SuspensionDistFL = Raw("CurrentPlayerTelemetry.mWheels01.mSuspensionDeflection");
					SuspensionDistFR = Raw("CurrentPlayerTelemetry.mWheels02.mSuspensionDeflection");
					SuspensionDistRL = Raw("CurrentPlayerTelemetry.mWheels03.mSuspensionDeflection");
					SuspensionDistRR = Raw("CurrentPlayerTelemetry.mWheels04.mSuspensionDeflection");
					if (H.N.Brake > 80.0)
					{
						ABSActive = (Raw("CurrentPlayerTelemetry.mWheels01.mBrakePressure")
								   + Raw("CurrentPlayerTelemetry.mWheels02.mBrakePressure")) * 100.0 < H.N.Brake - 1.0;
						break;
					}
					break;
				case GameId.GranTurismo7:
				case GameId.GranTurismoSport:
					SuspensionDistFL = Raw("Tire_SusHeight01");
					SuspensionDistFR = Raw("Tire_SusHeight02");
					SuspensionDistRL = Raw("Tire_SusHeight03");
					SuspensionDistRR = Raw("Tire_SusHeight04");
					break;
			}
			if (!flag)
				return;
			SuspensionVelFL = (SuspensionDistFL - SuspensionDistFLP) * FPS;
			SuspensionVelFR = (SuspensionDistFR - SuspensionDistFRP) * FPS;
			SuspensionVelRL = (SuspensionDistRL - SuspensionDistRLP) * FPS;
			SuspensionVelRR = (SuspensionDistRR - SuspensionDistRRP) * FPS;
		}

		internal ushort Rpms;

		// called from DataUpdate()
		internal void Refresh(BlekHapt bh, PluginManager pluginManager)
		{
			H = bh;
			PM = pluginManager;
			FPS = (double) PM.GetPropertyValue("DataCorePlugin.DataUpdateFps");
			Rpms = Convert.ToUInt16(0.5 + H.N.Rpms);
			SpeedMs = H.N.SpeedKmh * 0.277778;
			Accelerator = H.N.Throttle;
			Clutch = H.N.Clutch;
			Handbrake = H.N.Handbrake;
			if (CarInitCount < 2)
			{
				SuspensionDistFLP = SuspensionDistFL;
				SuspensionDistFRP = SuspensionDistFR;
				SuspensionDistRLP = SuspensionDistRL;
				SuspensionDistRRP = SuspensionDistRR;
				YawPrev = H.N.OrientationYaw;
				Yaw = H.N.OrientationYaw;
				RumbleLeftAvg = 0.0;
				RumbleRightAvg = 0.0;
			}
			YawPrev = Yaw;
			Yaw = -H.N.OrientationYaw;
			if (Yaw > 100.0 && YawPrev < -100.0)
				YawPrev += 360.0;
			else if (Yaw < -100.0 && YawPrev > 100.0)
				YawPrev -= 360.0;
			YawRate = (Yaw - YawPrev) * FPS;
			if (YawRateAvg != 0.0)
			{
				if (Math.Abs(YawRate) < Math.Abs(YawRateAvg * 1.25))
					YawRateAvg = (YawRateAvg + YawRate) * 0.5;
				else
					YawRateAvg *= 1.25;
			}
			else YawRateAvg = YawRate;
			++Acc0;
			Acc1 = Acc0 - 1;
			if (Acc0 >= AccSamples)
			{
				Acc0 = 0;
				Acc1 = AccSamples - 1;
			}
			AccHeave[Acc0] = H.N.AccelerationHeave.GetValueOrDefault();
			AccSurge[Acc0] = H.N.AccelerationSurge.GetValueOrDefault();
			AccSway[Acc0] = H.N.AccelerationSway.GetValueOrDefault();
			if (!H.N.AccelerationHeave.HasValue)
			{
				AccHeave[Acc0] = Raw("WorldSpeedY");
				AccHeave[Acc0] = (AccHeave[Acc0] - WorldSpeedY) * FPS;
				WorldSpeedY = Raw("WorldSpeedY");
			}
			AccHeave2S = (AccHeave[Acc0] + AccHeave[Acc1]) * 0.5;
			AccSurge2S = (AccSurge[Acc0] + AccSurge[Acc1]) * 0.5;
			AccSway2S = (AccSway[Acc0] + AccSway[Acc1]) * 0.5;
			JerkZ = (AccHeave[Acc0] - AccHeave[Acc1]) * FPS;
			JerkY = (AccSurge[Acc0] - AccSurge[Acc1]) * FPS;
			JerkX = (AccSway[Acc0] - AccSway[Acc1]) * FPS;
			double num1 = 1.0 / 16.0;
			double accSurgeAvg = AccSurgeAvg;
			AccHeaveAvg = 0.0;
			AccSurgeAvg = 0.0;
			AccSwayAvg = 0.0;
			for (int index = 0; index < AccSamples; ++index)
			{
				AccHeaveAvg += AccHeave[index];
				AccSurgeAvg += AccSurge[index];
				AccSwayAvg += AccSway[index];
			}
			AccHeaveAvg *= num1;
			AccSurgeAvg *= num1;
			AccSwayAvg *= num1;
			JerkYAvg = (AccSurgeAvg - accSurgeAvg) * FPS;
			AccHeaveAbs = Math.Abs(AccHeave[Acc0]);
			InvAccSurgeAvg = AccSurgeAvg != 0.0 ? 1.0 / AccSurgeAvg : 0.0;
			MotionPitch = MotionPitchOffset + 100.0 * Math.Pow(Math.Abs(MotionPitchMult * H.N.OrientationPitch) * 0.01, 1.0 / MotionPitchGamma);
			MotionRoll = MotionRollOffset + 100.0 * Math.Pow(Math.Abs(MotionRollMult * H.N.OrientationRoll) * 0.01, 1.0 / MotionRollGamma);
			MotionYaw = MotionYawOffset + 100.0 * Math.Pow(Math.Abs(MotionYawMult * YawRateAvg) * 0.01, 1.0 / MotionYawGamma);
			MotionHeave = MotionHeaveOffset + 100.0 * Math.Pow(Math.Abs(MotionHeaveMult * AccHeave[Acc0]) * 0.01, 1.0 / MotionHeaveGamma);
			if (H.N.OrientationPitch < 0.0)
				MotionPitch = -MotionPitch;
			if (H.N.OrientationRoll < 0.0)
				MotionRoll = -MotionRoll;
			if (YawRateAvg < 0.0)
				MotionYaw = -MotionYaw;
			if (AccHeave[Acc0] < 0.0)
				MotionHeave = -MotionHeave;

			UpdateVehicle();
			Airborne = AccHeave2S < -2.0 || Math.Abs(H.N.OrientationRoll) > 60.0;
			Airborne = Airborne && SuspensionAll < 0.1;
			if (DateTime.Now.Ticks < FrameTimeTicks)	// long rollover?
				FrameCountTicks += (long.MaxValue - FrameTimeTicks) + DateTime.Now.Ticks;	// rollover
			else FrameCountTicks += DateTime.Now.Ticks - FrameTimeTicks;
			FrameTimeTicks = DateTime.Now.Ticks;
			if (FrameCountTicks > 864000000000L)
				FrameCountTicks = 0L;

			if (DateTime.Now.Ticks < ShiftTicks)	// long rollover?
				ShiftTicks = - (DateTime.Now.Ticks + (long.MaxValue - ShiftTicks));
			if (DateTime.Now.Ticks - ShiftTicks > H.Settings.DownshiftDurationMs * 10000)
				Downshift = false;
			if (DateTime.Now.Ticks - ShiftTicks > H.Settings.UpshiftDurationMs * 10000)
				Upshift = false;
			DateTime now;
			if (H.Gdat.OldData.Gear != H.N.Gear)
			{
				if (Gear != 0)
					GearPrevious = Gear;
				Gear = !(H.N.Gear == "N")
						? (!(H.N.Gear == "R")
							? Convert.ToInt32(H.N.Gear)
							: -1)
						: 0;
				if (Gear != 0 && Gear != GearPrevious)
				{
					now = DateTime.Now;
					ShiftTicks = now.Ticks;
					if (Gear < GearPrevious)
						Downshift = true;
					else Upshift = true;
				}
			}
			ABSPulseInterval = 166666L * H.Settings.ABSPulseLength;
			if (ABSActive)
			{
				now = DateTime.Now;
				if (ABSTicks <= 0L)
					ABSTicks = now.Ticks;
				if (now.Ticks - ABSTicks < ABSPulseInterval)
					ABSPulse = 100.0;
				else if (now.Ticks - ABSTicks < ABSPauseInterval)
					ABSPulse = 0.0;
				else
				{
					ABSPulse = 100.0;
					ABSTicks = now.Ticks;
				}
			}
			else
			{
				ABSPulse = 0.0;
				ABSTicks = -1L;
			}
			SuspensionAccFLP = SuspensionAccFL;
			SuspensionAccFRP = SuspensionAccFR;
			SuspensionAccRLP = SuspensionAccRL;
			SuspensionAccRRP = SuspensionAccRR;
			SuspensionAccFL = (SuspensionVelFL - SuspensionVelFLP) * FPS;
			SuspensionAccFR = (SuspensionVelFR - SuspensionVelFRP) * FPS;
			SuspensionAccRL = (SuspensionVelRL - SuspensionVelRLP) * FPS;
			SuspensionAccRR = (SuspensionVelRR - SuspensionVelRRP) * FPS;
			SuspensionFL = 10.0 * SuspensionMult * SuspensionMultAll * 100.0 * Math.Pow(Math.Max(SuspensionVelFL, 0.0) * 0.01, 1.0 / (SuspensionGamma * SuspensionGammaAll));
			SuspensionFR = 10.0 * SuspensionMult * SuspensionMultAll * 100.0 * Math.Pow(Math.Max(SuspensionVelFR, 0.0) * 0.01, 1.0 / (SuspensionGamma * SuspensionGammaAll));
			SuspensionRL = 10.0 * SuspensionMult * SuspensionMultAll * 100.0 * Math.Pow(Math.Max(SuspensionVelRL, 0.0) * 0.01, 1.0 / (SuspensionGamma * SuspensionGammaAll));
			SuspensionRR = 10.0 * SuspensionMult * SuspensionMultAll * 100.0 * Math.Pow(Math.Max(SuspensionVelRR, 0.0) * 0.01, 1.0 / (SuspensionGamma * SuspensionGammaAll));
			SuspensionFront = SuspensionFL + SuspensionFR;
			SuspensionRear = SuspensionRL + SuspensionRR;
			SuspensionLeft = SuspensionFL + SuspensionRL;
			SuspensionRight = SuspensionFR + SuspensionRR;
			SuspensionAll = (SuspensionFL + SuspensionFR + SuspensionRL + SuspensionRR) * 0.5;
			SuspensionAccAll = (SuspensionAccFL + SuspensionAccFR + SuspensionAccRL + SuspensionAccRR) * 0.5;
			if (CarInitCount < 10 && FrameCountTicks % 2000000L <= 150000L)
			{
				SuspensionFL *= 0.1 * CarInitCount;
				SuspensionFR *= 0.1 * CarInitCount;
				SuspensionRL *= 0.1 * CarInitCount;
				SuspensionRR *= 0.1 * CarInitCount;
				++CarInitCount;
			}
			SuspensionFreq = H.N.SpeedKmh * (3.0 / 16.0);
			double num2 = 94.0 + 0.4 * SpeedMs;
			double num3 = 76.0 + 0.45 * SpeedMs;
			double num4 = 60.0 + 0.5 * SpeedMs;
			double num5 = 46.0 + 0.55 * SpeedMs;
			double num6 = 34.0 + 0.6 * SpeedMs;
			double num7 = 24.0 + 0.65 * SpeedMs;
			double num8 = 16.0 + 0.7 * SpeedMs;
			double num9 = 10.0 + 0.75 * SpeedMs;
			double num10 = 0.55 + 1.8 * AccHeaveAbs * (AccHeaveAbs + num2) / (num2 * num2);
			double num11 = 0.5 + 2.0 * AccHeaveAbs * (AccHeaveAbs + num3) / (num3 * num3);
			double num12 = 0.45 + 2.2 * AccHeaveAbs * (AccHeaveAbs + num4) / (num4 * num4);
			double num13 = 0.4 + 2.4 * AccHeaveAbs * (AccHeaveAbs + num5) / (num5 * num5);
			double num14 = 0.5 + 2.0 * AccHeaveAbs * (AccHeaveAbs + num6) / (num6 * num6);
			double num15 = 0.6 + 1.6 * AccHeaveAbs * (AccHeaveAbs + num7) / (num7 * num7);
			double num16 = 0.7 + 1.2 * AccHeaveAbs * (AccHeaveAbs + num8) / (num8 * num8);
			double num17 = 0.8 + 0.8 * AccHeaveAbs * (AccHeaveAbs + num9) / (num9 * num9);
			double num18 = RumbleMult * RumbleMultAll * (0.6 + SpeedMs * (90.0 - SpeedMs) * 0.0002);
			if (SuspensionFreq < 30.0)
			{
				if (SuspensionFreq < 20.0)
				{
					if (SuspensionFreq < 15.0)
					{
						if (SuspensionFreq < 10.0)
						{
							if (SuspensionFreq < 7.5)
							{
								if (SuspensionFreq < 3.75)
								{
									SuspensionFreq *= 4.0;
									SuspensionFreqRa = SuspensionFreq * 0.715;
									SuspensionFreqRb = SuspensionFreq * 1.0;
									SuspensionFreqRc = SuspensionFreq * 1.43;
									SuspensionFreqR1 = SuspensionFreq * 2.0;
									SuspensionFreqR2 = SuspensionFreq * 2.86;
									SuspensionFreqR3 = SuspensionFreq * 4.0;
									SuspensionFreqR4 = SuspensionFreq * 5.72;
									SuspensionFreqR5 = SuspensionFreq * 8.0;
									SuspensionMultRa = num10 * 0.5;
									SuspensionMultRb = num11 * 1.0;
									SuspensionMultRc = num12 * 0.5;
									SuspensionMultR1 = num13 * 0.8;
									SuspensionMultR2 = num14 * 0.25;
									SuspensionMultR3 = num15 * 0.6;
									SuspensionMultR4 = num16 * 0.125;
									SuspensionMultR5 = num17 * 0.4;
									SuspensionRumbleMultRa = num18 * 0.0;
									SuspensionRumbleMultRb = num18 * 2.0;
									SuspensionRumbleMultRc = num18 * 0.0;
									SuspensionRumbleMultR1 = num18 * 1.5;
									SuspensionRumbleMultR2 = num18 * 0.0;
									SuspensionRumbleMultR3 = num18 * 1.0;
									SuspensionRumbleMultR4 = num18 * 0.0;
									SuspensionRumbleMultR5 = num18 * 0.5;
								}
								else
								{
									SuspensionFreq *= 2.0;
									SuspensionFreqRa = SuspensionFreq * 0.715;
									SuspensionFreqRb = SuspensionFreq * 1.0;
									SuspensionFreqRc = SuspensionFreq * 1.43;
									SuspensionFreqR1 = SuspensionFreq * 2.0;
									SuspensionFreqR2 = SuspensionFreq * 2.86;
									SuspensionFreqR3 = SuspensionFreq * 4.0;
									SuspensionFreqR4 = SuspensionFreq * 5.72;
									SuspensionFreqR5 = SuspensionFreq * 8.0;
									SuspensionMultRa = num10 * 0.5;
									SuspensionMultRb = num11 * 1.0;
									SuspensionMultRc = num12 * 0.5;
									SuspensionMultR1 = num13 * 0.8;
									SuspensionMultR2 = num14 * 0.25;
									SuspensionMultR3 = num15 * 0.6;
									SuspensionMultR4 = num16 * 0.125;
									SuspensionMultR5 = num17 * 0.4;
									SuspensionRumbleMultRa = num18 * 0.0;
									SuspensionRumbleMultRb = num18 * 2.0;
									SuspensionRumbleMultRc = num18 * 0.0;
									SuspensionRumbleMultR1 = num18 * 1.5;
									SuspensionRumbleMultR2 = num18 * 0.0;
									SuspensionRumbleMultR3 = num18 * 1.0;
									SuspensionRumbleMultR4 = num18 * 0.0;
									SuspensionRumbleMultR5 = num18 * 0.5;
								}
							}
							else
							{
								SuspensionFreqRa = SuspensionFreq * 1.0;
								SuspensionFreqRb = SuspensionFreq * 1.43;
								SuspensionFreqRc = SuspensionFreq * 2.0;
								SuspensionFreqR1 = SuspensionFreq * 2.86;
								SuspensionFreqR2 = SuspensionFreq * 4.0;
								SuspensionFreqR3 = SuspensionFreq * 5.72;
								SuspensionFreqR4 = SuspensionFreq * 8.0;
								SuspensionFreqR5 = SuspensionFreq * 11.44;
								SuspensionMultRa = num10 * 1.0;
								SuspensionMultRb = num11 * 0.5;
								SuspensionMultRc = num12 * 0.8;
								SuspensionMultR1 = num13 * 0.25;
								SuspensionMultR2 = num14 * 0.6;
								SuspensionMultR3 = num15 * 0.125;
								SuspensionMultR4 = num16 * 0.4;
								SuspensionMultR5 = num17 * (1.0 / 16.0);
								SuspensionRumbleMultRa = num18 * 2.0;
								SuspensionRumbleMultRb = num18 * 0.0;
								SuspensionRumbleMultRc = num18 * 1.5;
								SuspensionRumbleMultR1 = num18 * 0.0;
								SuspensionRumbleMultR2 = num18 * 1.0;
								SuspensionRumbleMultR3 = num18 * 0.0;
								SuspensionRumbleMultR4 = num18 * 0.5;
								SuspensionRumbleMultR5 = num18 * 0.0;
							}
						}
						else
						{
							SuspensionFreqRa = SuspensionFreq * 0.715;
							SuspensionFreqRb = SuspensionFreq * 1.0;
							SuspensionFreqRc = SuspensionFreq * 1.43;
							SuspensionFreqR1 = SuspensionFreq * 2.0;
							SuspensionFreqR2 = SuspensionFreq * 2.86;
							SuspensionFreqR3 = SuspensionFreq * 4.0;
							SuspensionFreqR4 = SuspensionFreq * 5.72;
							SuspensionFreqR5 = SuspensionFreq * 8.0;
							SuspensionMultRa = num10 * 0.5;
							SuspensionMultRb = num11 * 1.0;
							SuspensionMultRc = num12 * 0.5;
							SuspensionMultR1 = num13 * 0.8;
							SuspensionMultR2 = num14 * 0.25;
							SuspensionMultR3 = num15 * 0.6;
							SuspensionMultR4 = num16 * 0.125;
							SuspensionMultR5 = num17 * 0.4;
							SuspensionRumbleMultRa = num18 * 0.0;
							SuspensionRumbleMultRb = num18 * 2.0;
							SuspensionRumbleMultRc = num18 * 0.0;
							SuspensionRumbleMultR1 = num18 * 1.5;
							SuspensionRumbleMultR2 = num18 * 0.0;
							SuspensionRumbleMultR3 = num18 * 1.0;
							SuspensionRumbleMultR4 = num18 * 0.0;
							SuspensionRumbleMultR5 = num18 * 0.5;
						}
					}
					else
					{
						SuspensionFreqRa = SuspensionFreq * 0.5;
						SuspensionFreqRb = SuspensionFreq * 0.715;
						SuspensionFreqRc = SuspensionFreq * 1.0;
						SuspensionFreqR1 = SuspensionFreq * 1.43;
						SuspensionFreqR2 = SuspensionFreq * 2.0;
						SuspensionFreqR3 = SuspensionFreq * 2.86;
						SuspensionFreqR4 = SuspensionFreq * 4.0;
						SuspensionFreqR5 = SuspensionFreq * 5.72;
						SuspensionMultRa = num10 * 0.8;
						SuspensionMultRb = num11 * 0.5;
						SuspensionMultRc = num12 * 1.0;
						SuspensionMultR1 = num13 * 0.5;
						SuspensionMultR2 = num14 * 0.8;
						SuspensionMultR3 = num15 * 0.25;
						SuspensionMultR4 = num16 * 0.6;
						SuspensionMultR5 = num17 * 0.125;
						SuspensionRumbleMultRa = num18 * 1.5;
						SuspensionRumbleMultRb = num18 * 0.0;
						SuspensionRumbleMultRc = num18 * 2.0;
						SuspensionRumbleMultR1 = num18 * 0.0;
						SuspensionRumbleMultR2 = num18 * 1.5;
						SuspensionRumbleMultR3 = num18 * 0.0;
						SuspensionRumbleMultR4 = num18 * 1.0;
						SuspensionRumbleMultR5 = num18 * 0.0;
					}
				}
				else
				{
					SuspensionFreqRa = SuspensionFreq * (143.0 / 400.0);
					SuspensionFreqRb = SuspensionFreq * 0.5;
					SuspensionFreqRc = SuspensionFreq * 0.715;
					SuspensionFreqR1 = SuspensionFreq * 1.0;
					SuspensionFreqR2 = SuspensionFreq * 1.43;
					SuspensionFreqR3 = SuspensionFreq * 2.0;
					SuspensionFreqR4 = SuspensionFreq * 2.86;
					SuspensionFreqR5 = SuspensionFreq * 4.0;
					SuspensionMultRa = num10 * 0.25;
					SuspensionMultRb = num11 * 0.8;
					SuspensionMultRc = num12 * 0.5;
					SuspensionMultR1 = num13 * 1.0;
					SuspensionMultR2 = num14 * 0.5;
					SuspensionMultR3 = num15 * 0.8;
					SuspensionMultR4 = num16 * 0.25;
					SuspensionMultR5 = num17 * 0.6;
					SuspensionRumbleMultRa = num18 * 0.0;
					SuspensionRumbleMultRb = num18 * 1.5;
					SuspensionRumbleMultRc = num18 * 0.0;
					SuspensionRumbleMultR1 = num18 * 2.0;
					SuspensionRumbleMultR2 = num18 * 0.0;
					SuspensionRumbleMultR3 = num18 * 1.5;
					SuspensionRumbleMultR4 = num18 * 0.0;
					SuspensionRumbleMultR5 = num18 * 1.0;
				}
			}
			else if (SuspensionFreq > 40.0)
			{
				if (SuspensionFreq > 60.0)
				{
					if (SuspensionFreq > 80.0)
					{
						if (SuspensionFreq > 120.0)
						{
							SuspensionFreqRa = SuspensionFreq * (1.0 / 16.0);
							SuspensionFreqRb = SuspensionFreq * 0.089375;
							SuspensionFreqRc = SuspensionFreq * 0.125;
							SuspensionFreqR1 = SuspensionFreq * (143.0 / 800.0);
							SuspensionFreqR2 = SuspensionFreq * 0.25;
							SuspensionFreqR3 = SuspensionFreq * (143.0 / 400.0);
							SuspensionFreqR4 = SuspensionFreq * 0.5;
							SuspensionFreqR5 = SuspensionFreq * 0.715;
							SuspensionMultRa = num10 * 0.2;
							SuspensionMultRb = num11 * (1.0 / 16.0);
							SuspensionMultRc = num12 * 0.4;
							SuspensionMultR1 = num13 * 0.125;
							SuspensionMultR2 = num14 * 0.6;
							SuspensionMultR3 = num15 * 0.25;
							SuspensionMultR4 = num16 * 0.8;
							SuspensionMultR5 = num17 * 0.5;
							SuspensionRumbleMultRa = num18 * 0.3;
							SuspensionRumbleMultRb = num18 * 0.0;
							SuspensionRumbleMultRc = num18 * 0.5;
							SuspensionRumbleMultR1 = num18 * 0.0;
							SuspensionRumbleMultR2 = num18 * 1.0;
							SuspensionRumbleMultR3 = num18 * 0.0;
							SuspensionRumbleMultR4 = num18 * 1.5;
							SuspensionRumbleMultR5 = num18 * 0.0;
						}
						else
						{
							SuspensionFreqRa = SuspensionFreq * 0.089375;
							SuspensionFreqRb = SuspensionFreq * 0.125;
							SuspensionFreqRc = SuspensionFreq * (143.0 / 800.0);
							SuspensionFreqR1 = SuspensionFreq * 0.25;
							SuspensionFreqR2 = SuspensionFreq * (143.0 / 400.0);
							SuspensionFreqR3 = SuspensionFreq * 0.5;
							SuspensionFreqR4 = SuspensionFreq * 0.715;
							SuspensionFreqR5 = SuspensionFreq * 1.0;
							SuspensionMultRa = num10 * (1.0 / 16.0);
							SuspensionMultRb = num11 * 0.4;
							SuspensionMultRc = num12 * 0.125;
							SuspensionMultR1 = num13 * 0.6;
							SuspensionMultR2 = num14 * 0.25;
							SuspensionMultR3 = num15 * 0.8;
							SuspensionMultR4 = num16 * 0.5;
							SuspensionMultR5 = num17 * 1.0;
							SuspensionRumbleMultRa = num18 * 0.0;
							SuspensionRumbleMultRb = num18 * 0.5;
							SuspensionRumbleMultRc = num18 * 0.0;
							SuspensionRumbleMultR1 = num18 * 1.0;
							SuspensionRumbleMultR2 = num18 * 0.0;
							SuspensionRumbleMultR3 = num18 * 1.5;
							SuspensionRumbleMultR4 = num18 * 0.0;
							SuspensionRumbleMultR5 = num18 * 2.0;
						}
					}
					else
					{
						SuspensionFreqRa = SuspensionFreq * 0.125;
						SuspensionFreqRb = SuspensionFreq * (143.0 / 800.0);
						SuspensionFreqRc = SuspensionFreq * 0.25;
						SuspensionFreqR1 = SuspensionFreq * (143.0 / 400.0);
						SuspensionFreqR2 = SuspensionFreq * 0.5;
						SuspensionFreqR3 = SuspensionFreq * 0.715;
						SuspensionFreqR4 = SuspensionFreq * 1.0;
						SuspensionFreqR5 = SuspensionFreq * 1.43;
						SuspensionMultRa = num10 * 0.4;
						SuspensionMultRb = num11 * 0.125;
						SuspensionMultRc = num12 * 0.6;
						SuspensionMultR1 = num13 * 0.25;
						SuspensionMultR2 = num14 * 0.8;
						SuspensionMultR3 = num15 * 0.5;
						SuspensionMultR4 = num16 * 1.0;
						SuspensionMultR5 = num17 * 0.5;
						SuspensionRumbleMultRa = num18 * 0.5;
						SuspensionRumbleMultRb = num18 * 0.0;
						SuspensionRumbleMultRc = num18 * 1.0;
						SuspensionRumbleMultR1 = num18 * 0.0;
						SuspensionRumbleMultR2 = num18 * 1.5;
						SuspensionRumbleMultR3 = num18 * 0.0;
						SuspensionRumbleMultR4 = num18 * 2.0;
						SuspensionRumbleMultR5 = num18 * 0.0;
					}
				}
				else
				{
					SuspensionFreqRa = SuspensionFreq * (143.0 / 800.0);
					SuspensionFreqRb = SuspensionFreq * 0.25;
					SuspensionFreqRc = SuspensionFreq * (143.0 / 400.0);
					SuspensionFreqR1 = SuspensionFreq * 0.5;
					SuspensionFreqR2 = SuspensionFreq * 0.715;
					SuspensionFreqR3 = SuspensionFreq * 1.0;
					SuspensionFreqR4 = SuspensionFreq * 1.43;
					SuspensionFreqR5 = SuspensionFreq * 2.0;
					SuspensionMultRa = num10 * 0.125;
					SuspensionMultRb = num11 * 0.6;
					SuspensionMultRc = num12 * 0.25;
					SuspensionMultR1 = num13 * 0.8;
					SuspensionMultR2 = num14 * 0.5;
					SuspensionMultR3 = num15 * 1.0;
					SuspensionMultR4 = num16 * 0.5;
					SuspensionMultR5 = num17 * 0.8;
					SuspensionRumbleMultRa = num18 * 0.0;
					SuspensionRumbleMultRb = num18 * 1.0;
					SuspensionRumbleMultRc = num18 * 0.0;
					SuspensionRumbleMultR1 = num18 * 1.5;
					SuspensionRumbleMultR2 = num18 * 0.0;
					SuspensionRumbleMultR3 = num18 * 2.0;
					SuspensionRumbleMultR4 = num18 * 0.0;
					SuspensionRumbleMultR5 = num18 * 1.5;
				}
			}
			else
			{
				SuspensionFreqRa = SuspensionFreq * 0.25;
				SuspensionFreqRb = SuspensionFreq * (143.0 / 400.0);
				SuspensionFreqRc = SuspensionFreq * 0.5;
				SuspensionFreqR1 = SuspensionFreq * 0.715;
				SuspensionFreqR2 = SuspensionFreq * 1.0;
				SuspensionFreqR3 = SuspensionFreq * 1.43;
				SuspensionFreqR4 = SuspensionFreq * 2.0;
				SuspensionFreqR5 = SuspensionFreq * 2.86;
				SuspensionMultRa = num10 * 0.6;
				SuspensionMultRb = num11 * 0.25;
				SuspensionMultRc = num12 * 0.8;
				SuspensionMultR1 = num13 * 0.5;
				SuspensionMultR2 = num14 * 1.0;
				SuspensionMultR3 = num15 * 0.5;
				SuspensionMultR4 = num16 * 0.8;
				SuspensionMultR5 = num17 * 0.25;
				SuspensionRumbleMultRa = num18 * 1.0;
				SuspensionRumbleMultRb = num18 * 0.0;
				SuspensionRumbleMultRc = num18 * 1.5;
				SuspensionRumbleMultR1 = num18 * 0.0;
				SuspensionRumbleMultR2 = num18 * 2.0;
				SuspensionRumbleMultR3 = num18 * 0.0;
				SuspensionRumbleMultR4 = num18 * 1.5;
				SuspensionRumbleMultR5 = num18 * 0.0;
			}

			// idle sniff
			if (20 > IdleSampleCount && 300 < H.N.Rpms && H.N.Rpms <= idleRPM * 1.1)
			{	// Refresh(): supposes that idleRPM is somewhat valid..??
				if (40.0 > Math.Abs(H.Gdat.OldData.Rpms - H.N.Rpms) * FPS)
				{	// Refresh(): averaging with previous average
					idleRPM = Convert.ToUInt16((1 + idleRPM + (int)H.N.Rpms) >> 1);
					++IdleSampleCount;								// Refresh(): increment if difference < 40
				}
				if (20 == IdleSampleCount && 0 == H.S.IdleRPM)	// Refresh(): change H.S.IdleRPM?
					H.S.Idle(idleRPM);							// Refresh() sniff: only if it was 0
			}
		}	// Refresh()
	}
}
