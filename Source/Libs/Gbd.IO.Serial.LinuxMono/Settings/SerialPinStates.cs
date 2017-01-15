﻿using System;
using Gbd.IO.Serial.Error;
using Gbd.IO.Serial.Base;
using Gbd.IO.Serial.Enums;

namespace Gbd.IO.Serial.LinuxMono.Settings
{
    /// <summary> Serial Port pin states. </summary>
    public class SerialPinStates : SerialPinStatesBase {

        /// <summary> The associated serial port. </summary>
        public SerialPort Port => _Port;

        protected SerialPort _Port;

        /// <summary>
        ///     OUTPUT: Gets or sets a value indicating whether the Request to Send (RTS) signal is
        ///     enabled during serial communication.
        /// </summary>
        /// <value> true if RTS enable, false if not. </value>
        public override bool Rts_Enable {
            get { return _Rts_Enable; }
            set {
                if ((_Port.SerialSettings.Handshake == Handshake.RequestToSend ||
                     _Port.SerialSettings.Handshake == Handshake.RequestToSendXOnXOff))
                    throw new InvalidOperationException(SR.CantSetRtsWithHandshaking.ToResValue());
                _Rts_Enable = value;
            }
        }

        protected bool _Rts_Enable;

        /// <summary> Default constructor. </summary>
        public SerialPinStates() { }

        /// <summary> Constructor. </summary>
        /// <param name="sport"> The serial port to associate with. </param>
        internal SerialPinStates(SerialPort sport) {
            _Port = sport;
        }

        private bool portopen() {
            if (_Port == null) return false;
            return _Port.IsOpen;
        }

        /// <summary> Reads the actual values from the port. </summary>
        public override void Read() {
            if (!portopen())
                throw new InvalidOperationException(SR.Port_not_open.ToResValue());
            _Port.monoserialsignal.Read();

            _Ring_Detect = _Port.monoserialsignal.Ring_Detect;
            _CD_Detect = _Port.monoserialsignal.CD_Detect;
            _CTS_Detect = _Port.monoserialsignal.CTS_Detect;
            _DSR_Detect = _Port.monoserialsignal.DSR_Detect;

            Dtr_Enable = _Port.monoserialsignal.Dtr_Enable;
            _Rts_Enable = _Port.monoserialsignal.Rts_Enable;
        }

        /// <summary> Writes cached values to the port. </summary>
        public override void Write() {
            if (!portopen())
                throw new InvalidOperationException(SR.Port_not_open.ToResValue());
            var tmprts = _Rts_Enable;
            if ((_Port.SerialSettings.Handshake == Handshake.RequestToSend ||
                 _Port.SerialSettings.Handshake == Handshake.RequestToSendXOnXOff))
                tmprts = false;
            _Port.monoserialsignal.Rts_Enable = tmprts;
            _Port.monoserialsignal.Dtr_Enable = Dtr_Enable;
            _Port.monoserialsignal.BreakState = BreakState;
            _Port.monoserialsignal.Write();
        }

        /// <summary> Copy. </summary>
        /// <param name="inp"> The input settings to copy. </param>
        /// <returns> A copy of this object. </returns>
        public SerialPinStates Copy(SerialPinStates inp) {
            var ret = new SerialPinStates();
            ret.Import(inp);
            return ret;
        }
    }
}
