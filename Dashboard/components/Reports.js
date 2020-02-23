import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import MUIDataTable from 'mui-datatables';
import {Select, Paper, InputLabel, MenuItem, Grid, Button, AppBar, Tab, Tabs, Radio, RadioGroup, FormHelperText, FormControlLabel, FormControl, FormLabel, TextField} from '@material-ui/core';
import { createMuiTheme, MuiThemeProvider } from '@material-ui/core';
import {FreeEDRIconButton} from '../components/FreeEDRIconButton.js';
import InsertDriveFileIcon from '@material-ui/icons/InsertDriveFile';
export class Reports extends Component {
    constructor(props){
		super(props);
		this.state = {
			value: 0
		};
		this.handleChange = this.handleChange.bind(this);
	};

	handleChange(event, value){
	    this.setState({
	    	 value: value 
		});
	}

	render(){
		const { site } = this.props;
		const { value } = this.state;
		const options = {
			filterType: 'dropdown',
			selectableRows: false,
			downloadOptions: {
				filename: 'RecentReports.csv', separator: ','
			},
			rowsPerPage: 3,
		    rowsPerPageOptions: [3,5,8],
		};
		const chipTheme = createMuiTheme({
			palette: {
			  primary: { main: '#11cb5f' },
			  secondary: { main: '#ff0000' },
			  error: { main: '#8c8c8c' }
			},
			typography: { useNextVariants: true },
		  });
		const data = [
			["Daily Event Log", "11-3-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_11_3_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_11_3_19_10_12.log"],
			["Daily IIS Log", "11-3-2019 10:10:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyIIS_11_3_19_10_10.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyIIS_11_3_19_10_10.log"],
			["Daily Audit Log", "11-3-2019 10:05:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyAudit_11_3_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyAudit_11_3_19_10_05.log"],
			["User Application Log", "11-1-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\userApp_11_1_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\userApp_11_1_19_10_12.log"],
			["Daily Event Log", "11-1-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_11_1_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_11_1_19_10_12.log"],
			["Daily Event Log", "11-1-2019 10:10:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_11_1_19_10_10.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_11_1_19_10_10.log"],
			["Daily IIS Log", "10-29-2019 10:10:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyIIS_10_29_19_10_10.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyIIS_10_29_19_10_10.log"],
			["Daily Audit Log", "10-29-2019 10:05:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyAudit_10_29_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyAudit_10_29_19_10_12.log"],
			["User Application Log", "10-29-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\userApp_10_29_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\userApp_10_29_19_10_12.log"],
			["Daily Event Log", "10-28-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_10_28_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_10_28_19_10_12.log"],
			["Daily IIS Log", "10-28-2019 10:10:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyIIS_10_28_19_10_10.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyIIS_10_28_19_10_10.log"],
			["Daily Audit Log", "10-28-2019 10:05:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyAudit_10_28_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyAudit_10_28_19_10_12.log"],
			["User Application Log", "10-28-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\userApp_10_28_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\userApp_10_28_19_10_12.log"],
			["Daily Event Log", "10-27-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_10_27_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_10_27_19_10_12.log"],
		];

		const eventData = [
			["Daily Event Log", "11-3-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_11_3_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_11_3_19_10_12.log"],
			["Daily Event Log", "11-1-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_11_1_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_11_1_19_10_12.log"],
			["Daily Event Log", "11-1-2019 10:10:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_11_1_19_10_10.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_11_1_19_10_10.log"],
			["Daily Event Log", "10-28-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_10_28_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_10_28_19_10_12.log"],
			["Daily Event Log", "10-27-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_10_27_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyEvent_10_27_19_10_12.log"],
		];

		const loggingData = [
			["Daily IIS Log", "11-3-2019 10:10:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyIIS_11_3_19_10_10.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyIIS_11_3_19_10_10.log"],
			["User Application Log", "11-1-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\userApp_11_1_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\userApp_11_1_19_10_12.log"],
			["Daily IIS Log", "10-29-2019 10:10:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyIIS_10_29_19_10_10.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyIIS_10_29_19_10_10.log"],
			["User Application Log", "10-29-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\userApp_10_29_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\userApp_10_29_19_10_12.log"],
			["Daily IIS Log", "10-28-2019 10:10:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyIIS_10_28_19_10_10.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\dailyIIS_10_28_19_10_10.log"],
			["User Application Log", "10-28-2019 10:12:00Z", "C:\\inetpub\\LogFiles\\wscvsm1\\userApp_10_28_19_10_12.log", "IncidentTeam", "C:\\inetpub\\LogFiles\\wscvsm1\\userApp_10_28_19_10_12.log"],
		];

		const deploymentData = [
		

		];

		const theme = {
			spacing: 8,
		};
		const columns = [
			{
				name: "Report",
				options:{
					filter: true,
					display: 'true'
				}
			},
			{
				name: "DateTime",
				options:{
					filter: true,
					display: 'true'
				}
			},
			{
				name: "File Location",
				options:{
					filter: false,
					display: 'false'
				}
			},
			{
				name: "Generated By",
				options: {
					filter: true,
					display: 'true'
				}
			},
			{
				name: "Download",
				options: {
					filter: false,
					display: 'true',
					sort: false,
					customBodyRender: (value, tableMeta, updateValue) => {
						return (
							<FreeEDRIconButton color="Primary" airaLabel="Download Report" icon="create"/>
						)
					}
				}
			}];
		return (
			<div id="descriptionContent">
				<h3> FreeEDR Senior Design Reports Page </h3>
				<AppBar position="static">
				<Tabs value={value} onChange={this.handleChange} aria-label="simple tabs example">
					<Tab label="Daily Event Report"/>
					<Tab label="Rule Deployment Report"/>
					<Tab label="Logging Reports"/>
					<Tab label="View Previous Reports"/>
				</Tabs>
				</AppBar>
				{value === 0 && <div id="content">
				<br></br>
				<MUIDataTable title={"Recent Event Reports"} data={eventData} columns={columns} options={options}/>
				<br></br>
				<h3>Generate Reports</h3>
				<Grid container spacing={24}>
					<Grid item xs={6} sm={6}>
					<Paper>
						<Button variant="contained" color="default">
							<InsertDriveFileIcon/>	Report 1
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 2
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 3
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 4
						</Button>
					</Paper>
					</Grid>
					<Grid item xs={6} sm={6}>
					<Paper>
						<Button variant="contained"	color="default">
							<InsertDriveFileIcon/>	Report 5
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 6
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 7
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 8
						</Button>
					</Paper>
					</Grid>
				</Grid>						
				</div>}
				{value === 1 && <div id="content">
				<br></br>
				<MUIDataTable title={"Recent Deployment Reports"} data={deploymentData} columns={columns} options={options}/>
				<br></br>
				<h3>Generate Reports</h3>
				<Grid container spacing={24}>
					<Grid item xs={6} sm={6}>
					<Paper>
						<Button variant="contained" color="default">
							<InsertDriveFileIcon/>	Report 1
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 2
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 3
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 4
						</Button>
					</Paper>
					</Grid>
					<Grid item xs={6} sm={6}>
					<Paper>
						<Button variant="contained"	color="default">
							<InsertDriveFileIcon/>	Report 5
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 6
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 7
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 8
						</Button>
					</Paper>
					</Grid>
				</Grid>	
					
					
				</div>}
				{value === 2 && <div id="content">
				<br></br>
				<MUIDataTable title={"Recent Logging Reports"} data={loggingData} columns={columns} options={options}/>
				<br></br>
				<h3>Generate Reports</h3>
				<Grid container spacing={24}>
					<Grid item xs={6} sm={6}>
					<Paper>
						<Button variant="contained" color="default">
							<InsertDriveFileIcon/>	Report 1
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 2
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 3
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 4
						</Button>
					</Paper>
					</Grid>
					<Grid item xs={6} sm={6}>
					<Paper>
						<Button variant="contained"	color="default">
							<InsertDriveFileIcon/>	Report 5
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 6
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 7
						</Button>
						<br></br> <br></br>
						<Button variant="contained"	color="default">
						<InsertDriveFileIcon/>	Report 8
						</Button>
					</Paper>
					</Grid>
				</Grid>		
				</div>}
				{value === 3 && <div id="content">
				<h4> Below is the list of reports that were recently generated by the system users.</h4>
				<h4> In order to download the reports, please double click on the intended icon in the row of the report.</h4>
				<MUIDataTable title={"Recent Reports"} data={data} columns={columns} options={options}/></div>}
			</div>
		)
	}

}