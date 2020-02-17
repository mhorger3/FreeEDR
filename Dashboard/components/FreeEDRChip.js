import React, { Component } from 'react';
import { Chip, MuiThemeProvider, createMuiTheme } from '@material-ui/core';

export class FreeEDRChip extends Component {
	render(){
		const { children, label, color } = this.props;
		return (
			<Chip label={label} color={color}> {children} </Chip>
		)
	}
}