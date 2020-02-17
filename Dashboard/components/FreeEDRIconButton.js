import React, { Component } from 'react';
import { Icon, IconButton, MuiThemeProvider, createMuiTheme } from '@material-ui/core';

export class FreeEDRIconButton extends Component {
	render(){
		const {children, icon, color, airaLabel, onClick} = this.props;

		return (
			<IconButton color={color} aira-label={airaLabel} onClick={onClick}>
				<Icon> {icon} </Icon>
			</IconButton>
		)
	}

}