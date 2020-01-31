import React from 'react';
import ReactDOM from 'react-dom'
import {AppDashboardMaintenance} from '../src/AppDashboard.Main-1.0.0.0.js';
import { BgDashboard } from '../components/BgDashboard.js';
ReactDOM.render(<BgDashboard subMenu={['Home', 'Reports', 'Audit', 'Request Support', 'Admin']}/>, document.getElementById('pageHeader'));
ReactDOM.render(<AppDashboardMaintenance/>, document.getElementById('main'));


