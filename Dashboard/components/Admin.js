import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import MUIDataTable from 'mui-datatables';
import {AppBar, Tab, Tabs} from '@material-ui/core';
import { createMuiTheme, MuiThemeProvider } from '@material-ui/core';
import {FreeEDRIconButton} from '../components/FreeEDRIconButton.js';
import {FreeEDRChip} from '../components/FreeEDRChip.js';
export class Admin extends Component {
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
				filename: 'PermissionMatrix.csv', separator: ','
			},
			rowsPerPage: 7,
		    rowsPerPageOptions: [5,7,9],
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
			["FreeEDR\\Workstation Employee", "true", "false", "false", "false", "false", "false", "false"],
			["FreeEDR\\Script Manager", "true", "true", "false", "true", "false", "true", "false"],
			["FreeEDR\\Incident Response Manager", "true", "true", "true", "true", "false", "true", "false"],
			["FreeEDR\\Incident Response Supporter", "true", "true", "true", "true", "false", "true", "false"],
			["FreeEDR\\System Auditor", "true", "true", "true", "true", "true", "false", "false"],
			["FreeEDR\\Dashboard Infrastructure Manager", "true", "true", "true", "true", "true", "true", "true"],
			["FreeEDR\\mhorger", "true", "true", "true", "true", "true", "true", "false"],
			["FreeEDR\\zsantoro", "true", "true", "true", "true", "true", "true", "false"],
			["FreeEDR\\bbolesta", "true", "true", "true", "true", "true", "true", "false"],
			["FreeEDR\\dkelly", "true", "true", "true", "true", "true", "true", "false"],
			["FreeEDR\\rfiers", "true", "true", "true", "true", "true", "true", "false"],

		];

		const columns = [
			{
				name: "Active Directory Group",
				options:{
					filter: true,
					display: 'true'
				}
			},
			{
				name: "Base_Dashboard_RW",
				options:{
					filter: true,
					display: 'true',
					customBodyRender: (value, tableMeta, updateValue) => {
						var checkedValue, color;
						if(value == "true"){
							color = "primary";
							return (
							    <MuiThemeProvider theme={chipTheme}>
								<FreeEDRChip label={value} color={color}/>
								</MuiThemeProvider>
							)
						} else {
							color = "secondary";
							return (
							    <MuiThemeProvider theme={chipTheme}>
								<FreeEDRChip label={value} color={color}/>
								</MuiThemeProvider>
							)
						}
					}
				}
			},
			{
				name: "Daily_Event_Log_RW",
				options:{
					filter: true,
					display: 'true',
					customBodyRender: (value, tableMeta, updateValue) => {
						var checkedValue, color;
						if(value == "true"){
							color = "primary";
							return (
							    <MuiThemeProvider theme={chipTheme}>
								<FreeEDRChip label={value} color={color}/>
								</MuiThemeProvider>
							)
						} else {
							color = "secondary";
							return (
							    <MuiThemeProvider theme={chipTheme}>
								<FreeEDRChip label={value} color={color}/>
								</MuiThemeProvider>
							)
						}
					}
				}
			},
			{
				name: "Daily_Audit_Log_RW",
				options:{
					filter: true,
					display: 'true',
					customBodyRender: (value, tableMeta, updateValue) => {
						var checkedValue, color;
						if(value == "true"){
							color = "primary";
							return (
							    <MuiThemeProvider theme={chipTheme}>
								<FreeEDRChip label={value} color={color}/>
								</MuiThemeProvider>
							)
						} else {
							color = "secondary";
							return (
							    <MuiThemeProvider theme={chipTheme}>
								<FreeEDRChip label={value} color={color}/>
								</MuiThemeProvider>
							)
						}
					}
				}
			},
			{
				name: "Daily_IIS_Log_RW",
				options:{
					filter: true,
					display: 'true',
					customBodyRender: (value, tableMeta, updateValue) => {
						var checkedValue, color;
						if(value == "true"){
							color = "primary";
							return (
							    <MuiThemeProvider theme={chipTheme}>
								<FreeEDRChip label={value} color={color}/>
								</MuiThemeProvider>
							)
						} else {
							color = "secondary";
							return (
							    <MuiThemeProvider theme={chipTheme}>
								<FreeEDRChip label={value} color={color}/>
								</MuiThemeProvider>
							)
						}
					}
				}
			},
			{
				name: "Dashboard_Audit_RW",
				options:{
					filter: true,
					display: 'true',
					customBodyRender: (value, tableMeta, updateValue) => {
						var checkedValue, color;
						if(value == "true"){
							color = "primary";
							return (
							    <MuiThemeProvider theme={chipTheme}>
								<FreeEDRChip label={value} color={color}/>
								</MuiThemeProvider>
							)
						} else {
							color = "secondary";
							return (
							    <MuiThemeProvider theme={chipTheme}>
								<FreeEDRChip label={value} color={color}/>
								</MuiThemeProvider>
							)
						}
					}
				}
			},
			{
				name: "Dashboard_Admin_RW",
				options:{
					filter: true,
					display: 'true',
					customBodyRender: (value, tableMeta, updateValue) => {
						var checkedValue, color;
						if(value == "true"){
							color = "primary";
							return (
							    <MuiThemeProvider theme={chipTheme}>
								<FreeEDRChip label={value} color={color}/>
								</MuiThemeProvider>
							)
						} else {
							color = "secondary";
							return (
							    <MuiThemeProvider theme={chipTheme}>
								<FreeEDRChip label={value} color={color}/>
								</MuiThemeProvider>
							)
						}
					}
				}
			},
			{
				name: "Modify",
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
				<h3> FreeEDR Senior Design Admin Page </h3>
				<AppBar position="static">
				<Tabs value={value} onChange={this.handleChange} aria-label="simple tabs example">
					<Tab label="View Permissions Matrix"/>
					<Tab label="View Rule Repository"/>
				</Tabs>
				</AppBar>
				{value === 0 && <div id="content">
				<MUIDataTable title={"Permissions Matrix"} data={data} columns={columns} options={options}/>	
				</div>}
				{value === 1 && <div id="content">
					<h2> VPN File: </h2><a href="./pfSense-UDP4-11916-dragon-config.ovpn"><h4>Click Here!</h4></a>
					<h2> VCenter: </h2> <h4>192.168.1.101</h4>
					<h2> User: </h2> <h4>seniordesign@vsphere.local</h4>
					<h2> Password: </h2> <h4>mummify lukewarm Winking frustrate_99*</h4>
					<h2> VLAN: </h2> <h4>10.0.20.0/24</h4>
				</div>}
			</div>
		)
	}

}