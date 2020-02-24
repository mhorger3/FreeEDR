import React, { Component } from 'react';
import ReactDOM from 'react-dom';
import axios from 'axios';
import MUIDataTable from 'mui-datatables';
import { Tabs, Tab, AppBar, MuiThemeProvider, createMuiTheme, BottomNavigation } from '@material-ui/core';
import { Toolbar, Typography, MenuItem, Select, FormControl, InputLabel, Input  } from '@material-ui/core';
import {List, Divider, ListItem, ListItemText, Drawer, Button} from '@material-ui/core';
import {Admin} from './Admin.js';
import {Audit} from './Audit.js';
import {Reports} from './Reports.js';
import {Support} from './Support.js';
import {AppDashboardMaintenance} from '../src/AppDashboard.Main-1.0.0.0.js';

export class BgDashboard extends Component {
	constructor(props){
        super();
		this.state = {
				value: 0,
				top: false,
				left: false,
				bottom: false,
				right: false,
				isLoaded: false,
				errors: null
		};
		this.handleChange = this.handleChange.bind(this);
        this.toggleDrawer = this.toggleDrawer.bind(this);
        this.getApplications();
	}

	toggleDrawer(side, open){
        // need to figure out which side menu option was launched to pass control to application
        // routing needs to go here
		this.setState({
			[side]: open,
		});
    };
    
    // call our service with the given AD group to get the list of applications they have
    getApplications(){
        this.setState({
            applications: [],
            isLoaded: true
        })
    }

	checkData(){
		this.handleClose();
    };
    
	handleChange(event, value){
	    this.setState({
	    	 value: value 
    	});
	};

	renderSite(text){
            // need to handle routing here
            switch (text) {
                case "Home":
                    ReactDOM.render(<AppDashboardMaintenance/>, document.getElementById('main'));
                    break;
                case "Reports":
                    ReactDOM.render(<Reports/>, document.getElementById('main'));
                    break;
                case "Audit":
                    ReactDOM.render(<Audit/>, document.getElementById('main'));
                    break;
                case "Request Support":
                    ReactDOM.render(<Support/>, document.getElementById('main'));
                    break;
                case "Admin":
                    ReactDOM.render(<Admin/>, document.getElementById('main'));
                    break;
                default:


            }
                
    }
    
	render(){
        const { value, isLoaded, applications } = this.state;
        const { subMenu } = this.state;
		var rows;
		const styles = {
			root: {
			  flexGrow: 1,
			},
			grow: {
			  flexGrow: 1,
			},
			list: {
				width: 'auto',
			},
		};

		const sideList = (
			<div style={styles.list}>
				<List>
				{this.props.subMenu.map((text, index) => (
					<ListItem button key={text} onClick={() => {this.renderSite(text)}}>
					<ListItemText primary={text} />
					</ListItem>
				))}
				</List>
				<Divider/>
			</div>
        ); 
        
        return (
            <div display={{ xs: 'none', xl: 'block' }}>
                <div style={styles.root}>
                    <AppBar style={{background: '#6C8B8C'}} position="static">
                        <Toolbar>
                            <Typography align='left' variant="h5" color="inherit" style={styles.grow}>
                                
                                <Button id="sideMenu" onClick={() => {this.toggleDrawer('left', 'true')}}><i class="material-icons" style={{color: "white"}}>menu</i></Button>
                                &nbsp;&nbsp; &nbsp;&nbsp; FreeEDR Reporting Dashboard
                            </Typography>							
                            <Typography variant="p" align='right' color="inherit" style={styles.grow}>
                                <i class="material-icons" style={{color: "white", display: "inline-flex", verticalAlign: "middle"}}>account_circle</i>
                                &nbsp;&nbsp;Welcome Matt Horger&nbsp;&nbsp;
                            </Typography> 
                        </Toolbar>
                </AppBar>
                </div>
                <div id="mainContainer">
                    <Drawer open={this.state.left} onClose={() => {this.toggleDrawer('left', false)}}>
                        <div
                            tabIndex={0}
                            role="button"
                            onClick={() => {this.toggleDrawer('left', false)}}
                            onKeyDown={() => {this.toggleDrawer('left', false)}}
                        >
                        {sideList}
                        </div>
                    </Drawer>
                </div>
            </div>
        )
	}
}
