import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import MUIDataTable from 'mui-datatables';
import {AppBar, Tab, Tabs} from '@material-ui/core';
import { createMuiTheme, MuiThemeProvider } from '@material-ui/core';
import {FreeEDRIconButton} from '../components/FreeEDRIconButton.js';
export class Support extends Component {
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
		return (
			<div id="descriptionContent">
				<h3> FreeEDR Senior Design Support Page </h3>
				<AppBar position="static">
				<Tabs value={value} onChange={this.handleChange} aria-label="simple tabs example">
					<Tab label="Contact Incident Support"/>
					<Tab label="Support Staff Information"/>
					<Tab label="Submit a New Request"/>
				</Tabs>
				</AppBar>
				{value === 0 && <div id="content"></div>}
				{value === 1 && <div id="content">
					
				</div>}
				{value === 2 && <div id="content"></div>}
			</div>
		)
	}

}